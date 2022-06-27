using UnityEngine;
using ZestCore.Utility;
using DG.Tweening;
using ZestGames;

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
        private readonly int _civillianIndexID = Animator.StringToHash("CivillianIndex");
        private readonly int _stopClappingID = Animator.StringToHash("StopClapping");
        private readonly int _getTakenID = Animator.StringToHash("GetTaken");
        private readonly int _getThrownID = Animator.StringToHash("GetThrown");

        private readonly int _withBagDefendID = 1;
        private readonly int _withoutBagDefendID = 2;

        public void Init(CharacterBase character)
        {
            _civillian = GetComponent<Civillian>();
            _animator = GetComponent<Animator>();
            SetCivillianIndex();

            _civillian.OnIdle += Idle;
            _civillian.OnWalk += Walk;
            _civillian.OnDefeated += Defeated;
            _civillian.OnGetTakenToAmbulance += GetTaken;

            _civillian.OnGetAttacked += Defend;
            _civillian.OnRescued += Rescued;

            _civillian.OnGetThrown += GetThrown;
        }

        private void OnDisable()
        {
            if (!_civillian) return;

            _civillian.OnIdle -= Idle;
            _civillian.OnWalk -= Walk;
            _civillian.OnDefeated -= Defeated;
            _civillian.OnGetTakenToAmbulance -= GetTaken;

            _civillian.OnGetAttacked -= Defend;
            _civillian.OnRescued -= Rescued;

            _civillian.OnGetThrown -= GetThrown;

            transform.DOKill();
        }

        #region INTERFACE FUNCTIONS
        public void Idle() => _animator.SetBool(_walkID, false);
        public void Walk() => _animator.SetBool(_walkID, true);
        public void Defeated() => _animator.SetTrigger(_defeatedID);
        #endregion

        private void SetCivillianIndex()
        {
            if (_civillian.Type == Enums.CivillianType.WithBag)
                _animator.SetInteger(_civillianIndexID, _withBagDefendID);
            else if (_civillian.Type == Enums.CivillianType.WithoutBag)
                _animator.SetInteger(_civillianIndexID, _withoutBagDefendID);
        }
        private void Defend(Criminal ignoreThis)
        {
            //SetCivillianIndex();
            _animator.SetTrigger(_defendID);
        }
        private void GetTaken(Ambulance ambulance) => _animator.SetTrigger(_getTakenID);
        private void GetThrown()
        {
            _animator.SetTrigger(_getThrownID);

            transform.DOMove(transform.position - (transform.forward * 0.7f), 1f).OnComplete(() => {
                //transform.DOMove(transform.position + (transform.forward * 3), 1f);
                transform.DOJump(transform.position + (transform.forward * 2.75f), 0.75f, 1, 1f);
            });
        }
        private void Rescued()
        {
            if (_civillian.IsDefeated) return;
            _animator.SetTrigger(_rescuedID);
            Delayer.DoActionAfterDelay(this, _civillian.ClapTime, () => _animator.SetTrigger(_stopClappingID));
        }
    }
}
