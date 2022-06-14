using System;
using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class Civillian : CharacterBase
    {
        [SerializeField] private Enums.CivillianType type;
        public Enums.CivillianType Type => type;

        private Criminal _attackingCriminal;
        public Criminal AttackingCriminal => _attackingCriminal;

        #region EVENTS
        public Action OnRescued;
        public Action<Criminal> OnGetAttacked;
        #endregion

        public bool IsBeingAttacked { get; private set; }

        private void OnEnable()
        {
            CharacterManager.AddCivillian(this);
            Init();
            IsBeingAttacked = false;

            OnGetAttacked += GetAttacked;
        }

        private void OnDisable()
        {
            CharacterManager.RemoveCivillian(this);

            OnGetAttacked -= GetAttacked;
        }

        private void GetAttacked(Criminal criminal)
        {
            IsBeingAttacked = true;
            _attackingCriminal = criminal;
        }
    }
}
