using UnityEngine;

namespace SuperheroIdle
{
    public interface ICharacterMovement
    {
        public void Init(CharacterBase character);
        public void Motor();
        public bool IsMoving { get; }
    }
}
