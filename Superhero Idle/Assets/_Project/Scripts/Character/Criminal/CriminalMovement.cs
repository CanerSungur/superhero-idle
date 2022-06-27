using UnityEngine;
using UnityEngine.AI;
using ZestCore.Ai;
using ZestCore.Utility;
using ZestGames;
using DG.Tweening;

namespace SuperheroIdle
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CriminalMovement : MonoBehaviour, ICharacterMovement
    {
        private Criminal _criminal;
        private NavMeshAgent _agent;
        private Vector3 _currentWalkTargetPosition;
        private Transform _currentAttackTransform;
        private bool _targetReached = false;

        [Header("-- SETUP --")]
        [SerializeField] private float speed = 1f;
        private float _runAwaySpeed, _runToAttackSpeed;

        public bool IsMoving => _agent.velocity.magnitude >= 0.1f;

        private bool _isBeingTakenToPoliceCar, _hasReachedToPoliceCar, _searchPath, _hasPath;

        public void Init(CharacterBase character)
        {
            _criminal = GetComponent<Criminal>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;
            _agent.radius = 0.25f;
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            _runToAttackSpeed = speed * 1.5f;
            _runAwaySpeed = speed * 2f;
            _isBeingTakenToPoliceCar = _hasReachedToPoliceCar = _searchPath = _hasPath = false;

            StartSearchingPath();

            _criminal.OnProceedAttack += ProceedAttack;
            _criminal.OnAttack += Stop;
            _criminal.OnDefeated += Defeated;
            _criminal.OnRunAway += RunAway;
            _criminal.OnGetTakenToPoliceCar += GetTakenToPoliceCar;
        }

        private void OnDisable()
        {
            if (!_criminal) return;

            _criminal.OnProceedAttack -= ProceedAttack;
            _criminal.OnAttack -= Stop;
            _criminal.OnDefeated -= Defeated;
            _criminal.OnRunAway -= RunAway;
            _criminal.OnGetTakenToPoliceCar -= GetTakenToPoliceCar;
        }

        private void Update()
        {
            if (_criminal.IsDefeated && _isBeingTakenToPoliceCar)
            {
                if (Operation.IsTargetReached(transform, _currentWalkTargetPosition, 1f))
                    GetThrownToThePoliceCar();
            }

            if (_criminal.IsDefeated || GameManager.GameState == Enums.GameState.GameEnded) return;

            if (_criminal.IsAttacking)
                ChaseClosestAttackTransform();

            #region SEARCH PATH IF WANTED
            if (_searchPath)
            {
                if (RandomNavMeshLocation() && !_hasPath)
                {
                    Motor();
                    _searchPath = false;
                    _hasPath = true;
                }
            }
            #endregion

            #region RE-SEARCH PATH AFTER REACHING
            if (Operation.IsTargetReached(transform, _currentWalkTargetPosition, 3f) && !_targetReached)
            {
                _targetReached = true;
                if (_criminal.IsAttacking)
                {
                    _criminal.OnAttack?.Invoke();
                    ActivateRelevantVictim();
                }
                else
                {
                    _hasPath = false;
                    _criminal.OnIdle?.Invoke();
                    StartSearchingPath();
                }
            }
            #endregion
        }

        #region MOVEMENT FUNCTIONS
        private void StartSearchingPath() => _searchPath = true;
        public void Motor()
        {
            _targetReached = false;
            _agent.SetDestination(_currentWalkTargetPosition);
            _criminal.OnWalk?.Invoke();
        }
        private void ChaseClosestAttackTransform()
        {
            _agent.SetDestination(_currentAttackTransform.position);
            _currentWalkTargetPosition = _currentAttackTransform.position;

            if (_targetReached) // To avoid rotation mix up when criminal is attacking.
            {
                transform.LookAt(_currentAttackTransform);
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            }
        }
        public void Stop()
        {
            _targetReached = true;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }
        private void RunAway(bool ignoreThis)
        {
            _agent.speed = _runAwaySpeed;
            _targetReached = false;
            _agent.isStopped = false;

            Motor();
        }
        #endregion

        private void ProceedAttack(Enums.CriminalAttackType attackType)
        {
            if (attackType == Enums.CriminalAttackType.Civillian)
                _currentAttackTransform = _criminal.AttackDecider.TargetCivillian.transform;
            else if (attackType == Enums.CriminalAttackType.ATM)
                _currentAttackTransform = _criminal.AttackDecider.TargetAtm.transform;

            _agent.speed = _runToAttackSpeed;
        }
        private void ActivateRelevantVictim()
        {
            if (_criminal.AttackDecider.AttackType == Enums.CriminalAttackType.Civillian)
            {
                _criminal.AttackDecider.TargetCivillian.OnGetAttacked?.Invoke(_criminal);
            }
            else if (_criminal.AttackDecider.AttackType == Enums.CriminalAttackType.ATM)
                _criminal.AttackDecider.TargetAtm.OnGetAttacked?.Invoke();
        }
        private bool RandomNavMeshLocation()
        {
            Vector3 randomPosition = RNG.RandomPointInBounds(_criminal.BelongedPhase.Collider.bounds);

            NavMeshPath _path = new NavMeshPath();
            _agent.CalculatePath(randomPosition, _path);

            if (_path.status == NavMeshPathStatus.PathInvalid)
                return false;
            else
            {
                _currentWalkTargetPosition = randomPosition;
                return true;
            }
        }
        private void Defeated()
        {
            Stop();
        }

        private void GetTakenToPoliceCar(PoliceCar policeCar)
        {
            if (RNG.RollDice(50))
                _currentWalkTargetPosition = policeCar.LeftDropTransform.position;
            else
                _currentWalkTargetPosition = policeCar.RightDropTransform.position;

            _isBeingTakenToPoliceCar = true;

            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            _targetReached = false;
            _agent.isStopped = false;
            _targetReached = false;
            _agent.SetDestination(_currentWalkTargetPosition);
        }
        private void GetThrownToThePoliceCar()
        {
            if (!_hasReachedToPoliceCar)
            {
                Stop();

                _hasReachedToPoliceCar = true;
                transform.DOLookAt(_criminal.ActivatedPoliceCar.transform.position, 2f, AxisConstraint.Y, Vector3.up).OnComplete(() =>
                {
                    for (int i = 0; i < _criminal.ActivatedPoliceCar.PoliceMen.Count; i++)
                        _criminal.ActivatedPoliceCar.PoliceMen[i].OnDropCriminal?.Invoke();

                    _isBeingTakenToPoliceCar = false;
                    _criminal.OnGetThrown?.Invoke();

                    Delayer.DoActionAfterDelay(this, 2f, () => gameObject.SetActive(false));
                });
            }
        }
    }
}
