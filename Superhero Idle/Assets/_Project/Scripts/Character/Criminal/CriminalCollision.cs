using System.Collections;
using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class CriminalCollision : MonoBehaviour
    {
        private Criminal _criminal;
        
        [Header("-- FIGHT SETUP --")]
        [SerializeField] private float fightDuration = 2f;
        private float _runningAwayFightDuration, _currentFightDuration;
        private bool _fightStarted = false;
        private IEnumerator _getDefeatedEnum;

        public void Init(Criminal criminal)
        {
            _criminal = criminal;
            _runningAwayFightDuration = fightDuration * 0.75f;
            _currentFightDuration = fightDuration;
            _fightStarted = false;
            _getDefeatedEnum = GetDefeated();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !_fightStarted && (_criminal.AttackStarted || _criminal.RunningAway) && player.StateController.CurrentState == Enums.PlayerState.Hero)
            {
                StartCoroutine(_getDefeatedEnum);
                PlayerEvents.OnStartFighting?.Invoke(_criminal);
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

                _criminal.OnRunAway?.Invoke(false);// false means crime is a fail.
                _currentFightDuration = _runningAwayFightDuration;
            }
        }

        private IEnumerator GetDefeated()
        {
            _fightStarted = true;

            yield return new WaitForSeconds(_currentFightDuration);

            _criminal.OnDefeated?.Invoke();
            PeopleEvents.OnCriminalDecreased?.Invoke();

            if (_criminal.AttackType == Enums.CriminalAttackType.Civillian)
                _criminal.TargetCivillian.OnRescued?.Invoke();
            else if (_criminal.AttackType == Enums.CriminalAttackType.ATM)
                _criminal.TargetAtm.OnRescued?.Invoke();

            PlayerEvents.OnStopFighting?.Invoke(_criminal);

            _fightStarted = false;
        }
    }
}
