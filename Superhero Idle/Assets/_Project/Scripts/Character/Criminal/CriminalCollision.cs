using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class CriminalCollision : MonoBehaviour
    {
        private Criminal _criminal;
        [SerializeField] private float fightDuration = 2f;
        private float _timer;
        private bool _fightStarted = false;

        public void Init(Criminal criminal)
        {
            _criminal = criminal;
            _fightStarted = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && !_fightStarted && _criminal.AttackStarted && player.StateController.CurrentState == Enums.PlayerState.Hero)
            {
                _fightStarted = true;
                _timer = fightDuration;
                PlayerEvents.OnStartFighting?.Invoke(_criminal);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out Player player) && _fightStarted)
            {
                _fightStarted = false;
                PlayerEvents.OnStopFighting?.Invoke(_criminal);
               
                // TOOD:
                // Add criminal run away.
            }
        }

        private void Update()
        {
            if (_fightStarted)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    GetDefeated();
                    _fightStarted = false;
                }
            }
        }

        private void GetDefeated()
        {
            _criminal.OnDefeated?.Invoke();
            PeopleEvents.OnCriminalDecreased?.Invoke();

            if (_criminal.AttackType == Enums.CriminalAttackType.Civillian)
                _criminal.TargetCivillian.OnRescued?.Invoke();
            else if (_criminal.AttackType == Enums.CriminalAttackType.ATM)
                _criminal.TargetAtm.OnRescued?.Invoke();

            PlayerEvents.OnStopFighting?.Invoke(_criminal);
        }
    }
}
