using UnityEngine;
using UnityEngine.AI;
using ZestCore.Ai;
using ZestCore.Utility;

namespace SuperheroIdle
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class CriminalMovement : MonoBehaviour, ICharacterMovement
    {
        private Criminal _criminal;
        private NavMeshAgent _agent;
        private Vector3 _currentTargetPosition;
        private bool _targetReached = false;
        private readonly float _randomWalkPosRadius = 50f;

        [Header("-- SETUP --")]
        [SerializeField] private float speed = 1f;
        private float _runAwaySpeed, _runToAttackSpeed;

        public bool IsMoving => _agent.velocity.magnitude >= 0.1f;

        public void Init(CharacterBase character)
        {
            _criminal = GetComponent<Criminal>();
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = speed;
            _runToAttackSpeed = speed * 1.5f;
            _runAwaySpeed = speed * 2f;

            _currentTargetPosition = RandomNavMeshLocation(_randomWalkPosRadius);
            Motor();

            _criminal.OnDecideToAttack += DecidedToAttack;
            _criminal.OnAttack += Stop;
            _criminal.OnDefeated += Defeated;
            _criminal.OnRunAway += RunAway;
        }

        private void OnDisable()
        {
            if (!_criminal) return;

            _criminal.OnDecideToAttack -= DecidedToAttack;
            _criminal.OnAttack -= Stop;
            _criminal.OnDefeated -= Defeated;
            _criminal.OnRunAway -= RunAway;
        }

        private void Update()
        {
            if (_criminal.IsAttacking)
                ChaseClosestCivillian();

            if (Operation.IsTargetReached(transform, _currentTargetPosition, 3f) && !_targetReached)
            {
                _targetReached = true;
                if (_criminal.IsAttacking)
                {
                    _criminal.OnAttack?.Invoke();
                    _criminal.TargetCivillian.OnGetAttacked?.Invoke(_criminal);
                }
                else
                {
                    _criminal.OnIdle?.Invoke();

                    Delayer.DoActionAfterDelay(this, 3f, () =>
                    {
                        _currentTargetPosition = RandomNavMeshLocation(_randomWalkPosRadius);
                        Motor();
                    });
                }
            }
        }

        public void Motor()
        {
            _targetReached = false;
            _agent.SetDestination(_currentTargetPosition);
            _criminal.OnWalk?.Invoke();
        }
        private void ChaseClosestCivillian()
        {
            _agent.SetDestination(_criminal.TargetCivillian.transform.position);
            _currentTargetPosition = _criminal.TargetCivillian.transform.position;
            transform.LookAt(_criminal.TargetCivillian.transform);
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

            _currentTargetPosition = RandomNavMeshLocation(_randomWalkPosRadius);
            Motor();
        }
        private void DecidedToAttack() => _agent.speed = _runToAttackSpeed;
        private void Defeated()
        {
            Debug.Log("Defeated Criminal!");
        }
    }
}
