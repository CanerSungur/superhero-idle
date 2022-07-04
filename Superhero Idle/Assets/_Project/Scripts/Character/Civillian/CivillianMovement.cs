using UnityEngine;
using UnityEngine.AI;
using ZestCore.Ai;
using ZestCore.Utility;
using ZestGames;
using DG.Tweening;

namespace SuperheroIdle
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CivillianMovement : MonoBehaviour, ICharacterMovement
    {
        private Civillian _civillian;
        private NavMeshAgent _agent;
        private Vector3 _currentTargetPosition;
        private bool _targetReached = false;

        [Header("-- SETUP --")]
        [SerializeField] private float speed = 1f;
        private readonly float _takenToAmbulanceSpeed = 5f;
        private float _currentSpeed;

        public bool IsMoving => _agent.velocity.magnitude >= 0.1f;
        private bool _isBeingTakenToAmbulance, _hasReachedToAmbulance, _searchPath, _hasPath = false;

        public void Init(CharacterBase characterBase)
        {
            _currentSpeed = speed;

            _civillian = GetComponent<Civillian>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _currentSpeed;
            _isBeingTakenToAmbulance = _hasReachedToAmbulance = _searchPath = _hasPath = false;

            StartSearchingPath();

            _civillian.OnDefeated += Defeated;
            _civillian.OnRescued += Rescued;
            _civillian.OnGetTakenToAmbulance += GetTakenToAmbulance;
        }

        private void OnDisable()
        {
            if (!_civillian) return;
            _civillian.OnDefeated -= Defeated;
            _civillian.OnRescued -= Rescued;
            _civillian.OnGetTakenToAmbulance -= GetTakenToAmbulance;
        }

        private void Update()
        {
            if (_civillian.IsDefeated && _isBeingTakenToAmbulance)
            {
                if (Operation.IsTargetReached(transform, _currentTargetPosition, 1f))
                    GetThrownToAmbulance();
            }

            if (_civillian.IsDefeated || GameManager.GameState == Enums.GameState.GameEnded) return;

            if (_civillian.IsBeingAttacked)
                transform.LookAt(_civillian.AttackingCriminal.transform);

            #region SEARCH PATH IF WANTED
            if (_searchPath)
            {
                if (RandomNavmeshLocation() && !_hasPath)
                {
                    Motor();
                    _searchPath = false;
                    _hasPath = true;
                }
            }
            #endregion

            #region RE-SEARCH PATH AFTER REACHING
            if (Operation.IsTargetReached(transform, _currentTargetPosition, 2f) && !_targetReached)
            {
                _targetReached = true;
                _hasPath = false;
                _civillian.OnIdle?.Invoke();
                StartSearchingPath();
            }
            #endregion
        }

        private void StartSearchingPath() => _searchPath = true;
        public void Motor()
        {
            _targetReached = false;
            _agent.SetDestination(_currentTargetPosition);
            _civillian.OnWalk?.Invoke();
        }

        private bool RandomNavmeshLocation()
        {
            Vector3 randomPosition = RNG.RandomPointInBounds(_civillian.BelongedPhase.Collider.bounds);

            NavMeshPath _path = new NavMeshPath();
            _agent.CalculatePath(randomPosition, _path);

            if (_path.status == NavMeshPathStatus.PathInvalid)
                return false;
            else
            {
                _currentTargetPosition = randomPosition;
                return true;
            }
        }

        private void GetTakenToAmbulance(Ambulance ambulance)
        {
            _currentTargetPosition = ambulance.DropTransform.position;
            _isBeingTakenToAmbulance = true;

            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            _targetReached = false;
            _agent.isStopped = false;
            _targetReached = false;
            _agent.SetDestination(_currentTargetPosition);
        }
        private void GetThrownToAmbulance()
        {
            if (!_hasReachedToAmbulance)
            {
                _civillian.ActivatedAmbulance.OnOpenDoor?.Invoke();

                Stop();

                _hasReachedToAmbulance = true;
                transform.DOLookAt(_civillian.ActivatedAmbulance.transform.position, 2f, AxisConstraint.Y, Vector3.up).OnComplete(() =>
                {
                    for (int i = 0; i < _civillian.ActivatedAmbulance.Medics.Count; i++)
                        _civillian.ActivatedAmbulance.Medics[i].OnDropCivillian?.Invoke();

                    _isBeingTakenToAmbulance = false;
                    _civillian.OnGetThrown?.Invoke();

                    Delayer.DoActionAfterDelay(this, 2f, () =>{
                        _civillian.ActivatedAmbulance.OnCloseDoor?.Invoke();
                        gameObject.SetActive(false);
                    });
                });
            }
        }
        public void Stop()
        {
            _targetReached = true;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }
        private void Rescued()
        {
            if (_civillian.IsDefeated) return;

            Delayer.DoActionAfterDelay(this, _civillian.ClapTime, () =>
            {
                _targetReached = false;
                _agent.isStopped = false;

                StartSearchingPath();
            });
        }
        private void Defeated()
        {
            _currentSpeed = _takenToAmbulanceSpeed;
            _agent.speed = _currentSpeed;
        }
    }
}
