using UnityEngine;

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

        private readonly int _withBagAttack = 1;
        private readonly int _withoutBagAttack = 2;

        public void Init(CharacterBase character)
        {
            _criminal = GetComponent<Criminal>();
            _animator = GetComponent<Animator>();

            _criminal.OnIdle += Idle;
            _criminal.OnWalk += Walk;
            _criminal.OnDefeated += Defeated;

            _criminal.OnDecideToAttack += DecideToAttack;
            _criminal.OnAttack += Attack;
            _criminal.OnRunAway += RunAway;
        }

        private void OnDisable()
        {
            if (!_criminal) return;

            _criminal.OnIdle -= Idle;
            _criminal.OnWalk -= Walk;
            _criminal.OnDefeated -= Defeated;
            
            _criminal.OnDecideToAttack -= DecideToAttack;
            _criminal.OnAttack -= Attack;
            _criminal.OnRunAway -= RunAway;
        }

        public void Idle() => _animator.SetBool(_walkID, false);
        public void Walk() => _animator.SetBool(_walkID, true);
        public void Defeated() => _animator.SetTrigger(_defeatedID);

        private void DecideToAttack() => _animator.SetTrigger(_decidedToAttackID);
        private void Attack() => _animator.SetTrigger(_attackID);
        private void RunAway() => _animator.SetTrigger(_runAwayID);
    }
}
