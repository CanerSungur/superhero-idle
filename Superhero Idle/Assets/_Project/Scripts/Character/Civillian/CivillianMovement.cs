using UnityEngine;
using UnityEngine.AI;
using ZestCore.Ai;
using ZestCore.Utility;
using ZestGames;

namespace SuperheroIdle
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CivillianMovement : MonoBehaviour, ICharacterMovement
    {
        private Civillian _civillian;
        private NavMeshAgent _agent;
        private Vector3 _currentTargetPosition;
        private bool _targetReached = false;
        private readonly float _randomWalkPosRadius = 10f;

        [Header("-- SETUP --")]
        [SerializeField] private float speed = 1f;

        public bool IsMoving => _agent.velocity.magnitude >= 0.1f;
        private bool _searchPath, _hasPath = false;

        public void Init(CharacterBase characterBase)
        {
            _civillian = GetComponent<Civillian>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;
            _searchPath = _hasPath = false;

            StartSearchingPath();
            //_currentTargetPosition = RandomNavmeshLocation(_randomWalkPosRadius);
            //Motor();

            //_civillian.OnGetAttacked += Stop;
            _civillian.OnDefeated += Defeated;
            _civillian.OnRescued += Rescued;
        }

        private void OnDisable()
        {
            if (!_civillian) return;
            //_civillian.OnGetAttacked -= Stop;
            _civillian.OnDefeated -= Defeated;
            _civillian.OnRescued -= Rescued;
        }

        private void Update()
        {
            if (_civillian.IsDefeated || GameManager.GameState == Enums.GameState.GameEnded) return;

            if (_civillian.IsBeingAttacked)
                transform.LookAt(_civillian.AttackingCriminal.transform);

            #region SEARCH PATH IF WANTED
            if (_searchPath)
            {
                if (RandomNavmeshLocation(_randomWalkPosRadius) && !_hasPath)
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

                //Delayer.DoActionAfterDelay(this, 3f, () => {
                //    _currentTargetPosition = RandomNavmeshLocation(_randomWalkPosRadius);
                //    Motor();
                //});
            }
            #endregion

            //#region START MOTOR AFTER NEW PATH
            //if (_hasPath && _targetReached)
            //    Motor();
            //#endregion

            //transform.rotation = Quaternion.LookRotation(_agent.velocity, Vector3.up);
        }

        private void StartSearchingPath() => _searchPath = true;
        public void Motor()
        {
            _targetReached = false;
            _agent.SetDestination(_currentTargetPosition);
            _civillian.OnWalk?.Invoke();
        }

        private bool RandomNavmeshLocation(float radius)
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


            //Vector3 randomDirection = Random.insideUnitSphere * radius;
            //randomDirection += transform.position;
            //NavMeshHit hit;

            //if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            //{
            //    NavMeshPath _path = new NavMeshPath();
            //    _agent.CalculatePath(_currentTargetPosition, _path);

            //    if (_path.status == NavMeshPathStatus.PathInvalid)
            //    {
            //        //Debug.Log("Coudn't find a way");
            //        return false;
            //    }
            //    else
            //    {
            //        _currentTargetPosition = hit.position;
            //        //Debug.Log("Found a way");
            //        return true;
            //    }
            //}
            //else
            //{
            //    //Debug.Log("Coudn't find a way");
            //    return false;
            //}
        }

        public void Stop()
        {
            _targetReached = true;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }
        private void Rescued()
        {
            Delayer.DoActionAfterDelay(this, _civillian.ClapTime, () => {
                _targetReached = false;
                _agent.isStopped = false;

                StartSearchingPath();
                //_currentTargetPosition = RandomNavmeshLocation(_randomWalkPosRadius);
                //Motor();
            });
        }
        private void Defeated()
        {
            Debug.Log("Civillian Defeated!");
        }
    }
}
