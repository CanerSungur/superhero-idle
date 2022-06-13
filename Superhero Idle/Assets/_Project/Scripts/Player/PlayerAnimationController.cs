using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private Player _player;
        private Animator _animator;

        #region ANIMATION PARAMETERS
        private readonly int _runID = Animator.StringToHash("Run");
        private readonly int _dieID = Animator.StringToHash("Die");
        private readonly int _runSpeedID = Animator.StringToHash("RunSpeed");
        #endregion

        private float _runSpeed;
        public float RunSpeed
        {
            get
            {
                if (_runSpeed <= 0) return 0;
                else if (_runSpeed >= 1) return 1;
                else return _runSpeed;
            }
            private set => _runSpeed = value;
        }

        public void Init(Player player)
        {
            _player = player;
            _animator = GetComponent<Animator>();

            PlayerEvents.OnMove += Run;
            PlayerEvents.OnIdle += Idle;
        }

        private void OnDisable()
        {
            PlayerEvents.OnMove -= Run;
            PlayerEvents.OnIdle -= Idle;
        }

        private void Run() => _animator.SetBool(_runID, true);
        private void Idle() => _animator.SetBool(_runID, false);
        private void Die() => _animator.SetTrigger(_dieID);
        private void UpdateRunSpeed() => _animator.SetFloat(_runSpeedID, RunSpeed);
    }
}
