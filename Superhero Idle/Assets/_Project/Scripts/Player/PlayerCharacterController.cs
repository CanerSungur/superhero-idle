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
        [SerializeField] private LayerMask walkableLayerMask;
        private Vector3 _playerVelocity;
        private const float GRAVITY_VALUE = -5f;
        private float _currentSpeed;
        private bool _startedMoving = false;
        
        private bool _enteringPhoneBooth = false;
        private PhoneBooth _activatedPhoneBooth = null;

        public float CivillianSpeed => DataManager.CurrentMovementSpeed;
        public float HeroSpeed => DataManager.CurrentMovementSpeed * 1.5f;
        public bool IsMoving => _player.InputHandler.InputValue != Vector3.zero;
        public bool IsGrounded => Physics.Raycast(_player.Collider.bounds.center, Vector3.down, _player.Collider.bounds.extents.y + 0.01f, walkableLayerMask);

        public void Init(Player player)
        {
            _player = player;
            _characterController = GetComponent<CharacterController>();

            _currentSpeed = CivillianSpeed;

            PlayerEvents.OnGoToPhoneBooth += GoToPhoneBooth;
            PlayerEvents.OnChangeToCivillian += SetCivillianSpeed;
            PlayerEvents.OnChangeToHero += SetHeroSpeed;
            PlayerEvents.OnSetCurrentMovementSpeed += UpdateMovementSpeed;
            UpgradeEvents.OnOpenUpgradeCanvas += RotateForUpgradeCamera;
        }

        private void OnDisable()
        {
            PlayerEvents.OnGoToPhoneBooth -= GoToPhoneBooth;
            PlayerEvents.OnChangeToCivillian -= SetCivillianSpeed;
            PlayerEvents.OnChangeToHero -= SetHeroSpeed;
            PlayerEvents.OnSetCurrentMovementSpeed -= UpdateMovementSpeed;
            UpgradeEvents.OnOpenUpgradeCanvas -= RotateForUpgradeCamera;
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
        private void GoToPhoneBooth(PhoneBooth phoneBooth)
        {
            _enteringPhoneBooth = true;
            _activatedPhoneBooth = phoneBooth;
            transform.DORotate(Vector3.zero, 0.5f);
            transform.DOMove(phoneBooth.EntryPosition, 0.5f).OnComplete(() => {
                EnterPhoneBooth();
            });
        }
        private void EnterPhoneBooth()
        {
            transform.DOMove(transform.position + (transform.forward * 1.5f), 0.5f);
            PlayerEvents.OnEnterPhoneBooth?.Invoke(_activatedPhoneBooth);
            Delayer.DoActionAfterDelay(this, _player.StateController.CurrentChangeTime, ExitPhoneBooth);
        }
        private void ExitPhoneBooth()
        {
            _enteringPhoneBooth = false;

            PlayerEvents.OnExitPhoneBooth?.Invoke(_activatedPhoneBooth);
            PlayerEvents.OnChangeToHero?.Invoke();
        }
        private void RotateForUpgradeCamera()
        {
            transform.rotation = Quaternion.identity;
        }
        private void SetCivillianSpeed() => _currentSpeed = CivillianSpeed;
        private void SetHeroSpeed() => _currentSpeed = HeroSpeed;
        private void UpdateMovementSpeed()
        {
            if (_player.StateController.CurrentState == Enums.PlayerState.Civillian)
                _currentSpeed = CivillianSpeed;
            else if (_player.StateController.CurrentState == Enums.PlayerState.Hero)
                _currentSpeed = HeroSpeed;
            else
                Debug.LogWarning("Invalid player state!");
        }
    }
}
