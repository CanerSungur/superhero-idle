using UnityEngine;
using ZestGames;
using DG.Tweening;

namespace SuperheroIdle
{
    public class CriminalAnimationController : MonoBehaviour, ICharacterAnimController
    {
        private Criminal _criminal;
        private Animator _animator;

        private readonly int _walkID = Animator.StringToHash("Walk");
        private readonly int _attackID = Animator.StringToHash("Attack");
        private readonly int _runAwayID = Animator.StringToHash("RunAway");
        private readonly int _defeatedID = Animator.StringToHash("Defeated");
        private readonly int _decidedToAttackID = Animator.StringToHash("DecidedToAttack");
        private readonly int _attackIndexID = Animator.StringToHash("AttackIndex");
        private readonly int _getDraggedID = Animator.StringToHash("GetDragged");
        private readonly int _getThrownID = Animator.StringToHash("GetThrown");

        private readonly int _withBagAttack = 1;
        private readonly int _withoutBagAttack = 2;
        private readonly int _atmAttack = 3;

        public void Init(CharacterBase character)
        {
            _criminal = GetComponent<Criminal>();
            _animator = GetComponent<Animator>();

            _criminal.OnIdle += Idle;
            _criminal.OnWalk += Walk;
            _criminal.OnDefeated += Defeated;
            _criminal.OnGetTakenToPoliceCar += GetDragged;

            _criminal.OnProceedAttack += SetAttackIndex;
            _criminal.OnAttack += Attack;
            _criminal.OnRunAway += RunAway;

            _criminal.OnGetThrown += GetThrown;
        }

        private void OnDisable()
        {
            if (!_criminal) return;

            _criminal.OnIdle -= Idle;
            _criminal.OnWalk -= Walk;
            _criminal.OnDefeated -= Defeated;
            _criminal.OnGetTakenToPoliceCar -= GetDragged;

            _criminal.OnProceedAttack -= SetAttackIndex;
            _criminal.OnAttack -= Attack;
            _criminal.OnRunAway -= RunAway;

            _criminal.OnGetThrown -= GetThrown;

            transform.DOKill();
        }

        public void Idle() => _animator.SetBool(_walkID, false);
        public void Walk() => _animator.SetBool(_walkID, true);
        public void Defeated() => _animator.SetTrigger(_defeatedID);

        private void GetDragged(PoliceCar ignoreThis) => _animator.SetTrigger(_getDraggedID);
        private void GetThrown()
        {
            _animator.SetTrigger(_getThrownID);

            transform.DOMove(transform.position - (transform.forward * 0.7f), 1f).OnComplete(() => {
                transform.DOJump(transform.position + (transform.forward * 2.75f), 0.75f, 1, 1f);
            });
        }
        private void SetAttackIndex(Enums.CriminalAttackType attackType)
        {
            if (attackType == Enums.CriminalAttackType.Civillian)
            {
                if (_criminal.AttackDecider.TargetCivillian.Type == Enums.CivillianType.WithBag)
                    _animator.SetInteger(_attackIndexID, _withBagAttack);
                else if (_criminal.AttackDecider.TargetCivillian.Type == Enums.CivillianType.WithoutBag)
                    _animator.SetInteger(_attackIndexID, _withoutBagAttack);
            }
            else if (attackType == Enums.CriminalAttackType.ATM)
                _animator.SetInteger(_attackIndexID, _atmAttack);

            _animator.SetTrigger(_decidedToAttackID);
        }

        private void Attack() => _animator.SetTrigger(_attackID);
        private void RunAway(bool ignoreThis) => _animator.SetTrigger(_runAwayID);

        #region ANIMATION EVENT FUNCTIONS
        public void HitAtm()
        {
            _criminal.AttackDecider.TargetAtm.OnGetHit?.Invoke();
        }
        #endregion
    }
}
