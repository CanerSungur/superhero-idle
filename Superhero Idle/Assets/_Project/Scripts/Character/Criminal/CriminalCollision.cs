using System.Collections;
using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class CriminalCollision : MonoBehaviour
    {
        private Criminal _criminal;

        [Header("-- FIGHT SETUP --")]
        private float _currentFightDuration;
        private bool _fightStarted = false;
        private IEnumerator _getDefeatedEnum;

        public bool FightStarted => _fightStarted;

        public void Init(Criminal criminal)
        {
            _criminal = criminal;
            _fightStarted = false;
            _getDefeatedEnum = GetDefeated();

            _currentFightDuration = _criminal.FightDuration;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !_fightStarted && (_criminal.AttackStarted || _criminal.RunningAway) && player.StateController.CurrentState == Enums.PlayerState.Hero)
            {
                StartCoroutine(_getDefeatedEnum);
                PlayerEvents.OnStartFighting?.Invoke(_criminal);
                AudioEvents.OnStartPunch?.Invoke();
                _criminal.Movement.Stop();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player) && _fightStarted)
            {
                _fightStarted = false;
                StopCoroutine(_getDefeatedEnum);
                _getDefeatedEnum = GetDefeated(); // Reset enumerator.

                PlayerEvents.OnStopFighting?.Invoke(_criminal);
                AudioEvents.OnStopPunch?.Invoke();

                _criminal.OnRunAway?.Invoke(false);// false means crime is a fail.
                _currentFightDuration = _criminal.RunningAwayFightDuration;
            }
        }

        private IEnumerator GetDefeated()
        {
            _fightStarted = true;

            yield return new WaitForSeconds(_currentFightDuration);

            _criminal.OnDefeated?.Invoke();
            PeopleEvents.OnCriminalDecreased?.Invoke();

            if (_criminal.AttackDecider.AttackType == Enums.CriminalAttackType.Civillian)
                _criminal.AttackDecider.TargetCivillian.OnRescued?.Invoke();
            else if (_criminal.AttackDecider.AttackType == Enums.CriminalAttackType.ATM)
                _criminal.AttackDecider.TargetAtm.OnRescued?.Invoke();

            PlayerEvents.OnStopFighting?.Invoke(_criminal);

            _fightStarted = false;
        }

        public void UpdateFightDuration() 
        {
            if (_criminal.RunningAway)
                _currentFightDuration = _criminal.RunningAwayFightDuration;
            else
                _currentFightDuration = _criminal.FightDuration;
        }
    }
}
