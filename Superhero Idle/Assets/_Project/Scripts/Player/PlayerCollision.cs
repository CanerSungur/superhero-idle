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
            if (other.TryGetComponent(out PhoneBooth phoneBooth))
            {
                if (phoneBooth.IsActivated && _player.StateController.CurrentState == Enums.PlayerState.Civillian)
                    PlayerEvents.OnGoToPhoneBooth?.Invoke(phoneBooth);

                if (phoneBooth.CanBeActivated && !phoneBooth.PlayerIsInArea)
                {
                    phoneBooth.PlayerIsInArea = true;
                    //phoneBooth.Activate();
                    _player.StartSpendingMoney(phoneBooth);
                }
            }

            if (other.TryGetComponent(out PhaseUnlocker phaseUnlocker) && !phaseUnlocker.PlayerIsInArea)
            {
                phaseUnlocker.PlayerIsInArea = true;
                _player.StartSpendingMoney(phaseUnlocker);
            }

            if (other.TryGetComponent(out UpgradeArea upgradeArea) && !upgradeArea.PlayerIsInArea)
            {
                upgradeArea.StartOpening();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PhoneBooth phoneBooth))
            {
                if (phoneBooth.IsActivated && phoneBooth.DoorIsOpen)
                     PlayerEvents.OnExitPhoneBoothSuccessfully?.Invoke(phoneBooth);

                if (phoneBooth.CanBeActivated && phoneBooth.PlayerIsInArea)
                {
                    phoneBooth.PlayerIsInArea = false;
                    _player.StopSpendingMoney(phoneBooth);
                }
            }

            if (other.TryGetComponent(out PhaseUnlocker phaseUnlocker) && phaseUnlocker.PlayerIsInArea)
            {
                phaseUnlocker.PlayerIsInArea = false;
                _player.StopSpendingMoney(phaseUnlocker);
            }

            if (other.TryGetComponent(out UpgradeArea upgradeArea) && upgradeArea.PlayerIsInArea)
            {
                upgradeArea.StopOpening();
            }
        }
    }
}
