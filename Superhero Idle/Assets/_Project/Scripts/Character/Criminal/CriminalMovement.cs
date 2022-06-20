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
        private readonly float _randomWalkPosRadius = 50f;

        [Header("-- SETUP --")]
        [SerializeField] private float speed = 1f;
        private float _runAwaySpeed, _runToAttackSpeed;

        public bool IsMoving => _agent.velocity.magnitude >= 0.1f;
        
        private bool _isBeingTakenToPoliceCar, _hasReachedToPoliceCar;

        public void Init(CharacterBase character)
        {
            _criminal = GetComponent<Criminal>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;
            _agent.radius = 0.5f;
            _runToAttackSpeed = speed * 1.5f;
            _runAwaySpeed = speed * 2f;
            _isBeingTakenToPoliceCar = _hasReachedToPoliceCar = false;

            _currentWalkTargetPosition = RandomNavMeshLocation(_randomWalkPosRadius);
            Motor();

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

                //if (!Operation.IsTargetReached(transform, _currentWalkTargetPosition, 1f))
                //{
                //    Navigation.MoveTransform(transform, _currentWalkTargetPosition, 1f);
                //    Navigation.LookAtTarget(transform, _currentWalkTargetPosition);
                //}
                //else
                //    GetThrownToThePoliceCar();
            }

            if (_criminal.IsDefeated || GameManager.GameState == Enums.GameState.GameEnded) return;

            if (_criminal.IsAttacking)
                ChaseClosestAttackTransform();

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
                    _criminal.OnIdle?.Invoke();

                    Delayer.DoActionAfterDelay(this, 3f, () =>
                    {
                        _currentWalkTargetPosition = RandomNavMeshLocation(_randomWalkPosRadius);
                        Motor();
                    });
                }
            }
        }

        #region MOVEMENT FUNCTIONS
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
        private void Stop()
        {
            _targetReached = true;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }
        private void RunAway()
        {
            _agent.speed = _runAwaySpeed;
            _targetReached = false;
            _agent.isStopped = false;

            _currentWalkTargetPosition = RandomNavMeshLocation(_randomWalkPosRadius);
            Motor();
        }
        #endregion


        private void ProceedAttack(Enums.CriminalAttackType attackType)
        {
            if (attackType == Enums.CriminalAttackType.Civillian)
                _currentAttackTransform = _criminal.TargetCivillian.transform;
            else if (attackType == Enums.CriminalAttackType.ATM)
                _currentAttackTransform = _criminal.TargetAtm.transform;

            _agent.speed = _runToAttackSpeed;
        }
        private void ActivateRelevantVictim()
        {
            if (_criminal.AttackType == Enums.CriminalAttackType.Civillian)
                _criminal.TargetCivillian.OnGetAttacked?.Invoke(_criminal);
            else if (_criminal.AttackType == Enums.CriminalAttackType.ATM)
                _criminal.TargetAtm.OnGetAttacked?.Invoke();
        }
        private Vector3 RandomNavMeshLocation(float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
                finalPosition = hit.position;
            return finalPosition;
        }
        private void Defeated()
        {
            Stop();
            //_agent.enabled = false;
            //Debug.Log("Defeated Criminal!");
        }

        private void GetTakenToPoliceCar(PoliceCar policeCar)
        {
            if (RNG.RollDice(50))
                _currentWalkTargetPosition = policeCar.LeftDropTransform.position;
            else
                _currentWalkTargetPosition = policeCar.RightDropTransform.position;

            _isBeingTakenToPoliceCar = true;

            _agent.radius = 1f;
            _targetReached = false;
            _agent.isStopped = false;
            Motor();

        }
        private void GetThrownToThePoliceCar()
        {
            if (!_hasReachedToPoliceCar)
            {
                Stop();

                _hasReachedToPoliceCar = true;
                transform.DOLookAt(_criminal.ActivatedPoliceCar.transform.position, 2f, AxisConstraint.Y, Vector3.up).OnComplete(() => {
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
