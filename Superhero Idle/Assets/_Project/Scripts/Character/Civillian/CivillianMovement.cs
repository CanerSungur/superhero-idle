using UnityEngine;
using UnityEngine.AI;
using ZestCore.Ai;
using ZestCore.Utility;

namespace SuperheroIdle
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CivillianMovement : MonoBehaviour, ICharacterMovement
    {
        private Civillian _civillian;
        private NavMeshAgent _agent;
        private Vector3 _currentTargetPosition;
        private bool _targetReached = false;
        private readonly float _randomWalkPosRadius = 50f;

        [Header("-- SETUP --")]
        [SerializeField] private float speed = 1f;

        public bool IsMoving => _agent.velocity.magnitude >= 0.1f;

        public void Init(CharacterBase characterBase)
        {
            _civillian = GetComponent<Civillian>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;

            _currentTargetPosition = RandomNavmeshLocation(_randomWalkPosRadius);
            Motor();

            _civillian.OnGetAttacked += Stop;
            _civillian.OnDefeated += Defeated;
            _civillian.OnRescued += Rescued;
        }

        private void OnDisable()
        {
            if (!_civillian) return;
            _civillian.OnGetAttacked -= Stop;
            _civillian.OnDefeated -= Defeated;
            _civillian.OnRescued -= Rescued;
        }

        private void Update()
        {
            if (_civillian.IsBeingAttacked)
                transform.LookAt(_civillian.AttackingCriminal.transform);

            if (Operation.IsTargetReached(transform, _currentTargetPosition, 0.5f) && !_targetReached)
            {
                _targetReached = true;
                _civillian.OnIdle?.Invoke();
             
                Delayer.DoActionAfterDelay(this, 3f, () => {
                    _currentTargetPosition = RandomNavmeshLocation(_randomWalkPosRadius);
                    Motor();
                });
            }
        }

        public void Motor()
        {
            _targetReached = false;
            _agent.SetDestination(_currentTargetPosition);
            _civillian.OnWalk?.Invoke();
        }

        private Vector3 RandomNavmeshLocation(float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            randomDirection += transform.position;
            NavMeshHit hit;
            Vector3 finalPosition = Vector3.zero;
            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
                finalPosition = hit.position;
            return finalPosition;
        }

        private void Stop(Criminal criminal)
        {
            _targetReached = true;
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }
        private void Rescued()
        {
            _targetReached = false;
            _agent.isStopped = false;
            
            _currentTargetPosition = RandomNavmeshLocation(_randomWalkPosRadius);
            Motor();
        }
        private void Defeated()
        {
            Debug.Log("Civillian Defeated!");
        }
    }
}
