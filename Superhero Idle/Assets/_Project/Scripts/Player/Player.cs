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
        #endregion

        private void Start()
        {
            CharacterController.Init(this);
        }
    }
}
