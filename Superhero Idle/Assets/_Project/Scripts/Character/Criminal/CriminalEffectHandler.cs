using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class CriminalEffectHandler : MonoBehaviour
    {
        private Criminal _criminal;
        private FightSmoke _fightSmoke;

        [Header("-- SETUP --")]
        [SerializeField] private ParticleSystem fightSmokePS;
        [SerializeField] private Animator exclamationMarkAnimator;

        public void Init(Criminal criminal)
        {
            _criminal = criminal;
            _fightSmoke = GetComponentInChildren<FightSmoke>();
            _fightSmoke.Init(_criminal);

            _criminal.OnProceedAttack += EnableExclamationMark;
            _criminal.OnAttack += DisableExclamationMark;
            _criminal.OnDefeated += DisableExclamationMark;
            _criminal.OnRunAway += DisableExclamationMark;

            PlayerEvents.OnStartFighting += StartFight;
            PlayerEvents.OnStopFighting += StopFight;
        }

        private void OnDisable()
        {
            _criminal.OnProceedAttack -= EnableExclamationMark;
            _criminal.OnAttack -= DisableExclamationMark;
            _criminal.OnDefeated -= DisableExclamationMark;
            _criminal.OnRunAway -= DisableExclamationMark;

            PlayerEvents.OnStartFighting -= StartFight;
            PlayerEvents.OnStopFighting -= StopFight;
        }

        private void StartFight(Criminal criminal)
        {
            if (criminal != _criminal) return;
            fightSmokePS.Play();
        }
        private void StopFight(Criminal criminal)
        {
            if (criminal != _criminal) return;
            fightSmokePS.Stop();
        }
        private void EnableExclamationMark(Enums.CriminalAttackType ignoreThis)
        {
            if (_criminal.IsDefeated) return;
            exclamationMarkAnimator.SetBool("Active", true);
        }

        private void DisableExclamationMark()
        {
            if (_criminal.IsDefeated) return;
            exclamationMarkAnimator.SetBool("Active", false);
        }
    }
}
