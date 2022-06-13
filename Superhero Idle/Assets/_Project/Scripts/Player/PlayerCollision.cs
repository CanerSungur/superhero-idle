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
            if (other.gameObject.layer == LayerMask.NameToLayer("PhoneBooth") && _player.StateController.CurrentState == Enums.PlayerState.Civillian)
            {
                PlayerEvents.OnChangeToHero?.Invoke();
            }
        }
    }
}
