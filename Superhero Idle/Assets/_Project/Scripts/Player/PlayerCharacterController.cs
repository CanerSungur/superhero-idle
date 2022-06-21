using UnityEngine;
using ZestGames;
using DG.Tweening;
using ZestCore.Utility;

namespace SuperheroIdle
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerCharacterController : MonoBehaviour, IPlayerMovement
    {
        private Player _player;
        private CharacterController _characterController;

        [Header("-- MOVEMENT SETUP --")]
        [SerializeField] private float civillianSpeed = 5f;
        [SerializeField] private float heroSpeed = 10f;
        [SerializeField] private LayerMask walkableLayerMask;
        private Vector3 _playerVelocity;
        private const float GRAVITY_VALUE = -5f;
        private float _currentSpeed;
        private bool _startedMoving = false;
        private bool _enteringPhoneBooth = false;

        public bool IsMoving => _player.InputHandler.InputValue != Vector3.zero;
        public bool IsGrounded => Physics.Raycast(_player.Collider.bounds.center, Vector3.down, _player.Collider.bounds.extents.y + 0.05f, walkableLayerMask);

        public void Init(Player player)
        {
            _player = player;
            _characterController = GetComponent<CharacterController>();

            _currentSpeed = civillianSpeed;

            PlayerEvents.OnGoToPhoneBooth += GoToPhoneBooth;
        }

        private void OnDisable()
        {
            PlayerEvents.OnGoToPhoneBooth -= GoToPhoneBooth;
        }

        private void Update()
        {
            if (_enteringPhoneBooth) return;

            #region NORMAL MOVEMENT
            if (IsGrounded && _playerVelocity.y < 0f)
                _playerVelocity.y = 0f;

            Motor();

            if (IsMoving)
            {
                transform.forward = _player.InputHandler.InputValue;

                if (!_startedMoving)
                {
                    PlayerEvents.OnMove?.Invoke();
                    _startedMoving = true;
                }
            }
            else
            {
                if (_startedMoving)
                {
                    PlayerEvents.OnIdle?.Invoke();
                    _startedMoving = false;
                }
            }

            _playerVelocity.y += GRAVITY_VALUE * Time.deltaTime;
            _characterController.Move(_playerVelocity * Time.deltaTime);
            #endregion
        }

        public void Motor() 
        {
            if (GameManager.GameState == Enums.GameState.Started)
                _characterController.Move(_currentSpeed * Time.deltaTime * _player.InputHandler.InputValue);
        }
        private void GoToPhoneBooth(Vector3 targetPosition)
        {
            _enteringPhoneBooth = true;
            transform.DORotate(Vector3.zero, 0.5f);
            transform.DOMove(targetPosition, 0.5f).OnComplete(() => {
                EnterPhoneBooth();
            });
        }
        private void EnterPhoneBooth()
        {
            transform.DOMove(transform.position + (transform.forward * 1.5f), 0.5f);
            PlayerEvents.OnEnterPhoneBooth?.Invoke();
            Delayer.DoActionAfterDelay(this, 3f, ExitPhoneBooth);
        }
        private void ExitPhoneBooth()
        {
            _enteringPhoneBooth = false;

            PlayerEvents.OnExitPhoneBooth?.Invoke();
            PlayerEvents.OnChangeToHero?.Invoke();
        }
    }
}
