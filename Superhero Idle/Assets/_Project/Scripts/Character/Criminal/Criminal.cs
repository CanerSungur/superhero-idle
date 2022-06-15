using UnityEngine;
using System;
using ZestGames;

namespace SuperheroIdle
{
    public class Criminal : CharacterBase
    {
        private CriminalCollision _collisionHandler;

        #region PROPERTIES
        private Civillian _targetCivillian;
        public Civillian TargetCivillian => _targetCivillian;
        private ATM _targetAtm;
        public ATM TargetAtm => _targetAtm;
        private Enums.CriminalAttackType _attackType;
        public Enums.CriminalAttackType AttackType => _attackType;
        #endregion

        #region EVENTS
        public Action OnAttack, OnRunAway, OnDecideToAttack;
        public Action<Enums.CriminalAttackType> OnProceedAttack;
        #endregion

        #region CONTROLS
        private bool _isAttacking = false;
        public bool IsAttacking => _isAttacking;
        #endregion

        private void OnEnable()
        {
            CharacterManager.AddCriminal(this);
            Init();
            _collisionHandler = GetComponent<CriminalCollision>();
            _collisionHandler.Init(this);

            OnDecideToAttack += ActivateAttack;
            OnDefeated += Defeated;
        }

        private void OnDisable()
        {
            CharacterManager.RemoveCriminal(this);
            OnDecideToAttack -= ActivateAttack;
            OnDefeated -= Defeated;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) && !_isAttacking)
            {
                DecideAttackType();

                _isAttacking = true;
                OnDecideToAttack?.Invoke();
            }
        }

        private void DecideAttackType()
        {
            int attackCount = Enum.GetNames(typeof(Enums.CriminalAttackType)).Length;
            _attackType = (Enums.CriminalAttackType)UnityEngine.Random.Range(0, attackCount);
            Debug.Log("Decided to: " + _attackType);
        }
        private void ActivateAttack()
        {
            if (_attackType == Enums.CriminalAttackType.Civillian)
                AttackCivillian();
            else if (_attackType == Enums.CriminalAttackType.ATM)
                AttackATM();
        }
        private void AttackCivillian()
        {
            _targetCivillian = FindClosestCivillian();
            //OnDecidedAttackCivillian?.Invoke();
            OnProceedAttack?.Invoke(_attackType);
        }
        private void AttackATM()
        {
            _targetAtm = FindClosestAtm();
            //OnDecidedAttackAtm?.Invoke();
            OnProceedAttack?.Invoke(_attackType);
        }
        private void Defeated()
        {
            PoliceManager.OnTakeCriminal?.Invoke(this);
            _isAttacking = false;
        }

        #region FIND FUNCTIONS
        private Civillian FindClosestCivillian()
        {
            if (CharacterManager.CivilliansInScene == null || CharacterManager.CivilliansInScene.Count == 0)
            {
                DecideAttackType();
                return null;
            }

            float shortestDistance = Mathf.Infinity;
            Civillian closestCivillian = null;

            for (int i = 0; i < CharacterManager.CivilliansInScene.Count; i++)
            {
                float distanceToTransform = (transform.position - CharacterManager.CivilliansInScene[i].transform.position).sqrMagnitude;
                if (distanceToTransform < shortestDistance && transform != CharacterManager.CivilliansInScene[i])
                {
                    shortestDistance = distanceToTransform;
                    closestCivillian = CharacterManager.CivilliansInScene[i];
                    CharacterManager.RemoveCivillian(CharacterManager.CivilliansInScene[i]);
                }
            }
            return closestCivillian;
        }
        private ATM FindClosestAtm()
        {
            if (CharacterManager.AtmsInScene == null || CharacterManager.AtmsInScene.Count == 0)
            {
                DecideAttackType();
                return null;
            }

            float shortestDistance = Mathf.Infinity;
            ATM closestAtm = null;

            for (int i = 0; i < CharacterManager.AtmsInScene.Count; i++)
            {
                float distanceToTransform = (transform.position - CharacterManager.AtmsInScene[i].transform.position).sqrMagnitude;
                if (distanceToTransform < shortestDistance && transform != CharacterManager.AtmsInScene[i])
                {
                    shortestDistance = distanceToTransform;
                    closestAtm = CharacterManager.AtmsInScene[i];
                    CharacterManager.RemoveATM(CharacterManager.AtmsInScene[i]);
                }
            }
            return closestAtm;
        }
        #endregion
    }
}
