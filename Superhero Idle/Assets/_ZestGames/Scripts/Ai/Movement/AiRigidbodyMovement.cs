using UnityEngine;

namespace ZestGames
{
    [RequireComponent(typeof(Rigidbody))]
    public class AiRigidbodyMovement : MonoBehaviour, IAiMovement
    {
        private Ai _ai;
        public bool IsMoving => _ai.Rigidbody.velocity.magnitude > 0.05f;
        public bool IsGrounded => _ai.IsGrounded;

        public void Init(Ai ai)
        {
            _ai = ai;
        }

        private void Update()
        {
            if (!_ai.CanMove) return;
            Motor();
        }

        public void Motor()
        {
            // Add rb movement.

        }
    }
}
