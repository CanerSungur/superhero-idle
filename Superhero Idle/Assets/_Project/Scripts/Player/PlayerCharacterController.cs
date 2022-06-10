using UnityEngine;
using ZestGames;

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

        public bool IsMoving => _player.InputHandler.InputValue != Vector3.zero;
        public bool IsGrounded => Physics.Raycast(_player.Collider.bounds.center, Vector3.down, _player.Collider.bounds.extents.y + 0.05f, walkableLayerMask);

        public void Init(Player player)
        {
            _player = player;
            _characterController = GetComponent<CharacterController>();

            _currentSpeed = civillianSpeed;

            PlayerEvents.OnChangeToCivillian += ChangeToCivillian;
            PlayerEvents.OnChangeToHero += ChangeToHero;
        }

        private void OnDisable()
        {
            PlayerEvents.OnChangeToCivillian -= ChangeToCivillian;
            PlayerEvents.OnChangeToHero -= ChangeToHero;
        }

        private void Update()
        {
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
        }

        public void Motor() 
        {
            if (GameManager.GameState == Enums.GameState.Started)
                _characterController.Move(_currentSpeed * Time.deltaTime * _player.InputHandler.InputValue);
        }

        private void ChangeToCivillian()
        {
            _currentSpeed = civillianSpeed;
        }

        private void ChangeToHero()
        {
            _currentSpeed = heroSpeed;
        }
    }
}
