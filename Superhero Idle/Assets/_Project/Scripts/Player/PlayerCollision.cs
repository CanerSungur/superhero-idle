using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class PlayerCollision : MonoBehaviour
    {
        private Player _player;

        public void Init(Player player)
        {
            _player = player;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PhoneBooth phoneBooth) && _player.StateController.CurrentState == Enums.PlayerState.Civillian)
            {
                PlayerEvents.OnGoToPhoneBooth?.Invoke(phoneBooth);
            }

            if (other.TryGetComponent(out PhaseUnlocker phaseUnlocker) && !phaseUnlocker.PlayerIsInArea)
            {
                phaseUnlocker.PlayerIsInArea = true;
                _player.StartSpendingMoney(phaseUnlocker);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PhoneBooth phoneBooth) && phoneBooth.DoorIsOpen)
            {
                phoneBooth.HeroExitSuccessfully();
            }

            if (other.TryGetComponent(out PhaseUnlocker phaseUnlocker) && phaseUnlocker.PlayerIsInArea)
            {
                phaseUnlocker.PlayerIsInArea = false;
                _player.StopSpendingMoney(phaseUnlocker);
            }
        }
    }
}
