using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SuperheroIdle
{
    public interface ICharacterAnimController
    {
        void Init(CharacterBase characterBase);
        void Idle();
        void Walk();
        void Defeated();
    }
}
