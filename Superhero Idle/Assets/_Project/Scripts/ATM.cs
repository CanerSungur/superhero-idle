using UnityEngine;
using System;

namespace SuperheroIdle
{
    public class ATM : MonoBehaviour
    {
        [Header("-- EFFECT SETUP --")]
        [SerializeField] private ParticleSystem smokePS;
        [SerializeField] private ParticleSystem moneyPS;

        #region EVENTS
        public Action OnGetAttacked, OnDefeated, OnGetHit, OnRescued;
        #endregion

        private void OnEnable()
        {
            CharacterManager.AddATM(this);

            OnGetAttacked += GetAttacked;
            OnDefeated += Defeated;
            OnGetHit += GetHit;
            OnRescued += Rescued;
        }

        private void OnDisable()
        {
            CharacterManager.RemoveATM(this);

            OnGetAttacked -= GetAttacked;
            OnDefeated -= Defeated;
            OnGetHit -= GetHit;
            OnRescued -= Rescued;
        }

        private void GetHit()
        {
            smokePS.Play();
            moneyPS.Play();
        }
        private void GetAttacked()
        {
            Debug.Log("Atm is getting attacked");
        }
        private void Defeated()
        {
            Debug.Log("Atm is defeated.");
        }
        private void Rescued() => CharacterManager.AddATM(this);
    }
}
