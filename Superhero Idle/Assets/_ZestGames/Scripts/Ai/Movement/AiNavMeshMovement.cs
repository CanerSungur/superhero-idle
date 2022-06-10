using UnityEngine;
using UnityEngine.AI;
using ZestCore.Ai;

namespace ZestGames
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AiNavMeshMovement : MonoBehaviour, IAiMovement
    {
        private Ai _ai;
        private Transform _currentTarget;
        private NavMeshAgent _agent;

        public bool IsMoving => _agent.velocity.magnitude > 0.05f;
        public bool IsGrounded => _ai.IsGrounded;

        public void Init(Ai ai)
        {
            _ai = ai;
            _currentTarget = ai.Target;
            _agent = GetComponent<NavMeshAgent>();
            _agent.speed = _ai.CurrentMovementSpeed;

            _ai.OnSetTarget += SetTarget;
        }

        private void OnDisable()
        {
            _ai.OnSetTarget -= SetTarget;
        }

        private void Update()
        {
            if (!_ai.CanMove || !_currentTarget) return;
            Motor();
        }

        public void Motor()
        {
            _agent.SetDestination(_currentTarget.position);

            if (Operation.IsTargetReached(transform, _currentTarget.position))
            {
                _currentTarget = null;
                _ai.OnIdle?.Invoke();
            }
        }

        private void SetTarget(Transform newTarget)
        {
            _currentTarget = newTarget;
        }
    }
}
