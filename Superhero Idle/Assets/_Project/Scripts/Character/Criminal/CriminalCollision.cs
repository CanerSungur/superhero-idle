using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class CriminalCollision : MonoBehaviour
    {
        private Criminal _criminal;

        public void Init(Criminal criminal)
        {
            _criminal = criminal;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Player player) && _criminal.IsAttacking)
            {
                _criminal.OnDefeated?.Invoke();

                if (_criminal.AttackType == Enums.CriminalAttackType.Civillian)
                    _criminal.TargetCivillian.OnRescued?.Invoke();
                else if (_criminal.AttackType == Enums.CriminalAttackType.ATM)
                    _criminal.TargetAtm.OnRescued?.Invoke();
            }
        }
    }
}