using System;
using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class Civillian : CharacterBase
    {
        [Header("-- SETUP --")]
        [SerializeField] private Enums.CivillianType type;
        [SerializeField] private float clapTime = 5f;
         
        #region EVENTS
        public Action OnRescued;
        public Action<Criminal> OnGetAttacked;
        #endregion

        #region PROPERTIES
        public Enums.CivillianType Type => type;
        public float ClapTime => clapTime;
        public Criminal AttackingCriminal { get; private set; }
        public bool IsBeingAttacked { get; private set; }
        #endregion

        private void OnEnable()
        {
            CharacterManager.AddCivillian(this);
            Init();
            IsBeingAttacked = false;

            OnGetAttacked += GetAttacked;
            OnRescued += Rescued;
        }

        private void OnDisable()
        {
            CharacterManager.RemoveCivillian(this);

            OnGetAttacked -= GetAttacked;
            OnRescued -= Rescued;
        }

        private void GetAttacked(Criminal criminal)
        {
            IsBeingAttacked = true;
            AttackingCriminal = criminal;
        }
        private void Rescued()
        {
            CharacterManager.AddCivillian(this);
            IsBeingAttacked = false;
        }
    }
}
