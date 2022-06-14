using System;
using UnityEngine;

namespace SuperheroIdle
{
    public abstract class CharacterBase : MonoBehaviour
    {
        #region COMPONENTS

        #endregion

        #region SCRIPT REFERENCES
        private ICharacterMovement _movement;
        public ICharacterMovement Movement => _movement == null ? _movement = GetComponent<ICharacterMovement>() : _movement;
        private ICharacterAnimController _animationController;
        public ICharacterAnimController AnimationController => _animationController == null ? _animationController = GetComponent<ICharacterAnimController>() : _animationController;
        #endregion

        #region EVENTS
        public Action OnIdle, OnWalk, OnDefeated;
        #endregion

        public bool IsDefeated { get; private set; }

        protected void Init()
        {
            IsDefeated = false;

            AnimationController.Init(this);
            Movement.Init(this);

            OnDefeated += Defeated;
        }

        private void OnDisable()
        {
            OnDefeated -= Defeated;
        }

        private void Defeated() => IsDefeated = true;
    }
}
