using System;
using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace SuperheroIdle
{
    public class Civillian : CharacterBase
    {
        private CivillianEffectHandler _effectHandler;

        [Header("-- SETUP --")]
        [SerializeField] private Enums.CivillianType type;
        [SerializeField] private float clapTime = 5f;
        private Transform _leftCarryPoint, _rightCarryPoint;

        #region EVENTS
        public Action OnRescued, OnGetThrown;
        public Action<Criminal> OnGetAttacked;
        public Action<Ambulance> OnGetTakenToAmbulance;
        #endregion

        #region PROPERTIES
        public Enums.CivillianType Type => type;
        public float ClapTime => clapTime;
        public Criminal AttackingCriminal { get; private set; }
        public bool IsBeingAttacked { get; private set; }
        public Phase BelongedPhase { get; private set; }
        public bool GettingTakenToAmbulance { get; private set; }
        public Ambulance ActivatedAmbulance { get; private set; }
        public Transform LeftCarryPoint => _leftCarryPoint;
        public Transform RightCarrypoint => _rightCarryPoint;
        #endregion

        private void OnEnable()
        {
            _rightCarryPoint = transform.GetChild(transform.childCount - 1);
            _leftCarryPoint = transform.GetChild(transform.childCount - 2);

            ActivatedAmbulance = null;
            IsBeingAttacked = false;

            //CharacterManager.AddCivillian(this);
            Init();
            _effectHandler = GetComponent<CivillianEffectHandler>();
            _effectHandler.Init(this);

            OnGetAttacked += GetAttacked;
            OnRescued += Rescued;
            OnDefeated += Defeated;
            OnGetTakenToAmbulance += GetTakenToAmbulance;
        }

        private void OnDisable()
        {
            if (BelongedPhase)
                BelongedPhase.RemoveActiveCivillian(this);
            //CharacterManager.RemoveCivillian(this);

            OnGetAttacked -= GetAttacked;
            OnRescued -= Rescued;
            OnDefeated -= Defeated;
            OnGetTakenToAmbulance -= GetTakenToAmbulance;
        }

        private void GetTakenToAmbulance(Ambulance ambulance) => GettingTakenToAmbulance = true;
        private void GetAttacked(Criminal criminal)
        {
            IsBeingAttacked = true;
            AttackingCriminal = criminal;
            Movement.Stop();
        }
        private void Rescued()
        {
            if (IsDefeated) return;
            BelongedPhase.AddActiveCivillian(this);
            //CharacterManager.AddCivillian(this);
            IsBeingAttacked = false;
        }
        private void Defeated()
        {
            IsBeingAttacked = GettingTakenToAmbulance = false;

            if (MedicManager.GetNextFreeAmbulance() != null)
            {
                Ambulance ambulance = MedicManager.GetNextFreeAmbulance();
                ActivatedAmbulance = ambulance;
                ambulance.StartTheCar(this);
            }
            else
            {
                Debug.Log("No available ambulance.");
                ActivatedAmbulance = null;
                DisableAfterSomeTime(5f);
            }
        }
        private void DisableAfterSomeTime(float time)
        {
            Delayer.DoActionAfterDelay(this, time, () => {
                if (ActivatedAmbulance)
                {
                    ActivatedAmbulance.ResetCar(this);
                    ActivatedAmbulance = null;
                }
                gameObject.SetActive(false);
            });
        }
        public void SetBelongedPhase(Phase phase) => BelongedPhase = phase;
    }
}
