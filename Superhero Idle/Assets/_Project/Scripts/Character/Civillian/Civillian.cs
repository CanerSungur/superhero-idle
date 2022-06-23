using System;
using UnityEngine;
using ZestCore.Utility;
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
        public Phase BelongedPhase { get; private set; }
        #endregion

        private void OnEnable()
        {
            //CharacterManager.AddCivillian(this);
            Init();
            IsBeingAttacked = false;

            OnGetAttacked += GetAttacked;
            OnRescued += Rescued;
            OnDefeated += Defeated;
        }

        private void OnDisable()
        {
            if (BelongedPhase)
                BelongedPhase.RemoveActiveCivillian(this);
            //CharacterManager.RemoveCivillian(this);

            OnGetAttacked -= GetAttacked;
            OnRescued -= Rescued;
            OnDefeated -= Defeated;
        }

        private void GetAttacked(Criminal criminal)
        {
            IsBeingAttacked = true;
            AttackingCriminal = criminal;
            Movement.Stop();
        }
        private void Rescued()
        {
            BelongedPhase.AddActiveCivillian(this);
            //CharacterManager.AddCivillian(this);
            IsBeingAttacked = false;
        }
        private void Defeated()
        {
            IsBeingAttacked = false;
            Delayer.DoActionAfterDelay(this, 10f, () => gameObject.SetActive(false));
        }

        public void SetBelongedPhase(Phase phase) => BelongedPhase = phase;
    }
}
