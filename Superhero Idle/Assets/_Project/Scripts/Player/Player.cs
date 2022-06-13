using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class Player : MonoBehaviour
    {
        #region COMPONENT REFERENCES
        private Collider _collider;
        public Collider Collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;
        #endregion

        #region SCRIPT REFERENCES
        private JoystickInput _inputHandler;
        public JoystickInput InputHandler => _inputHandler == null ? _inputHandler = GetComponent<JoystickInput>() : _inputHandler;
        private PlayerCharacterController _characterController;
        public PlayerCharacterController CharacterController => _characterController == null ? _characterController = GetComponent<PlayerCharacterController>() : _characterController;
        private PlayerAnimationController _animationController;
        public PlayerAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<PlayerAnimationController>() : _animationController;
        private PlayerCollision _collisionController;
        public PlayerCollision CollisionController => _collisionController == null ? _collisionController = GetComponent<PlayerCollision>() : _collisionController;
        private PlayerStateController _stateController;
        public PlayerStateController StateController => _stateController == null ? _stateController = GetComponent<PlayerStateController>() : _stateController;
        #endregion

        private void Start()
        {
            CharacterController.Init(this);
            AnimationController.Init(this);
            CollisionController.Init(this);
            StateController.Init(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (StateController.CurrentState == Enums.PlayerState.Civillian)
                    PlayerEvents.OnChangeToHero?.Invoke();
                else if (StateController.CurrentState == Enums.PlayerState.Hero)
                    PlayerEvents.OnChangeToCivillian?.Invoke();
            }
        }
    }
}
