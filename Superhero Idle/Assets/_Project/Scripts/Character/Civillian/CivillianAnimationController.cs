using UnityEngine;
using ZestCore.Utility;

namespace SuperheroIdle
{
    public class CivillianAnimationController : MonoBehaviour, ICharacterAnimController
    {
        private Civillian _civillian;
        private Animator _animator;

        private readonly int _walkID = Animator.StringToHash("Walk");
        private readonly int _defendID = Animator.StringToHash("Defend");
        private readonly int _rescuedID = Animator.StringToHash("Rescued");
        private readonly int _defeatedID = Animator.StringToHash("Defeated");
        private readonly int _defendIndexID = Animator.StringToHash("DefendIndex");
        private readonly int _stopClappingID = Animator.StringToHash("StopClapping");

        private readonly int _withBagDefendID = 1;
        private readonly int _withoutBagDefendID = 2;

        public void Init(CharacterBase character)
        {
            _civillian = GetComponent<Civillian>();
            _animator = GetComponent<Animator>();

            _civillian.OnIdle += Idle;
            _civillian.OnWalk += Walk;
            _civillian.OnDefeated += Defeated;

            _civillian.OnGetAttacked += Defend;
            _civillian.OnRescued += Rescued;
        }

        private void OnDisable()
        {
            if (!_civillian) return;

            _civillian.OnIdle -= Idle;
            _civillian.OnWalk -= Walk;
            _civillian.OnDefeated -= Defeated;
            _civillian.OnGetAttacked -= Defend;
            _civillian.OnRescued -= Rescued;
        }

        public void Idle() => _animator.SetBool(_walkID, false);
        public void Walk() => _animator.SetBool(_walkID, true);
        public void Defeated() => _animator.SetTrigger(_defeatedID);

        private void Defend(Criminal ignoreThis)
        {
            SetDefendIndex();
            _animator.SetTrigger(_defendID);
        }
        private void SetDefendIndex()
        {
            if (_civillian.Type == ZestGames.Enums.CivillianType.WithBag)
                _animator.SetInteger(_defendIndexID, _withBagDefendID);
            else if (_civillian.Type == ZestGames.Enums.CivillianType.WithoutBag)
                _animator.SetInteger(_defendIndexID, _withoutBagDefendID);
        }
        private void Rescued()
        {
            _animator.SetTrigger(_rescuedID);
            Delayer.DoActionAfterDelay(this, _civillian.ClapTime, () => _animator.SetTrigger(_stopClappingID));
        }
    }
}
