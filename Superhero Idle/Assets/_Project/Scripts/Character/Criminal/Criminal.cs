using UnityEngine;
using System;
using ZestGames;
using ZestCore.Utility;

namespace SuperheroIdle
{
    public class Criminal : CharacterBase
    {
        private CriminalCollision _collisionHandler;
        private CrimeDurationHandler _crimeDurationHandler;
        private CriminalEffectHandler _effectHandler;

        private Transform _leftCarryPoint, _rightCarryPoint;
        public Transform LeftCarryPoint => _leftCarryPoint;
        public Transform RightCarrypoint => _rightCarryPoint;
        public PoliceCar ActivatedPoliceCar { get; private set; }
        public Phase BelongedPhase { get; private set; }

        #region PROPERTIES
        private Civillian _targetCivillian;
        public Civillian TargetCivillian => _targetCivillian;
        private ATM _targetAtm;
        public ATM TargetAtm => _targetAtm;
        private Enums.CriminalAttackType _attackType;
        public Enums.CriminalAttackType AttackType => _attackType;
        #endregion

        #region EVENTS
        public Action OnAttack, OnDecideToAttack, OnGetThrown;
        public Action<Enums.CriminalAttackType> OnProceedAttack;
        public Action<PoliceCar> OnGetTakenToPoliceCar;
        public Action<bool> OnRunAway; // true is success, false is fail
        #endregion

        #region CONTROLS
        private bool _isAttacking = false;
        public bool IsAttacking => _isAttacking;
        public bool AttackStarted { get; private set; }
        public bool RunningAway { get; private set; }
        #endregion

        private void OnEnable()
        {
            _rightCarryPoint = transform.GetChild(transform.childCount - 1);
            _leftCarryPoint = transform.GetChild(transform.childCount - 2);

            ActivatedPoliceCar = null;
            AttackStarted = RunningAway = false;

            //CharacterManager.AddCriminal(this);
            Init();

            _collisionHandler = GetComponent<CriminalCollision>();
            _collisionHandler.Init(this);
            _crimeDurationHandler = GetComponent<CrimeDurationHandler>();
            //Delayer.DoActionAfterDelay(this, 0.5f, () => _crimeDurationHandler.Init(this));
            _crimeDurationHandler.Init(this);
            _effectHandler = GetComponent<CriminalEffectHandler>();
            //Delayer.DoActionAfterDelay(this, 0.2f, () => _effectHandler.Init(this));
            _effectHandler.Init(this);

            OnAttack += StartAttacking;
            OnDecideToAttack += ActivateAttack;
            OnDefeated += Defeated;
            OnRunAway += RunAway;
        }

        private void OnDisable()
        {
            if (BelongedPhase)
                BelongedPhase.RemoveActiveCriminal(this);
            //CharacterManager.RemoveCriminal(this);

            OnAttack -= StartAttacking;
            OnDecideToAttack -= ActivateAttack;
            OnDefeated -= Defeated;
            OnRunAway -= RunAway;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) && !_isAttacking)
            {
                DecideAttackType();

                OnDecideToAttack?.Invoke();
            }
        }

        private void DecideAttackType()
        {
            int attackCount = Enum.GetNames(typeof(Enums.CriminalAttackType)).Length;
            _attackType = (Enums.CriminalAttackType)UnityEngine.Random.Range(0, attackCount);
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
            //_targetCivillian = FindClosestCivillian();
            _targetCivillian = FindRandomCivillian();
            if (_targetCivillian == null) return;
            //OnDecidedAttackCivillian?.Invoke();
            OnProceedAttack?.Invoke(_attackType);
            _isAttacking = true;
        }
        private void AttackATM()
        {
            _targetAtm = FindClosestAtm();
            if (_targetAtm == null) return;
            //OnDecidedAttackAtm?.Invoke();
            OnProceedAttack?.Invoke(_attackType);
            _isAttacking = true;
        }
        private void Defeated()
        {
            _isAttacking = false;
            AttackStarted = RunningAway = false;

            if (PoliceManager.GetNextFreePoliceCar() != null)
            {
                PoliceCar policeCar = PoliceManager.GetNextFreePoliceCar();
                ActivatedPoliceCar = policeCar;
                policeCar.StartTheCar(this);
                if (ActivatedPoliceCar == null)
                    Debug.Log("No available police car.");
            }
            else
                Debug.Log("No available free police car.");
        }
        private void StartAttacking() => AttackStarted = true;
        private void RunAway(bool success)
        {
            _isAttacking = AttackStarted = false;
            RunningAway = true;

            if (success)
            {
                if (_attackType == Enums.CriminalAttackType.Civillian)
                    _targetCivillian.OnDefeated?.Invoke();
                else if (_attackType == Enums.CriminalAttackType.ATM)
                    _targetAtm.OnDefeated?.Invoke();

                PeopleEvents.OnCivillianDecreased?.Invoke();
            }
            else
            {
                if (_attackType == Enums.CriminalAttackType.Civillian)
                    _targetCivillian.OnRescued?.Invoke();
                else if (_attackType == Enums.CriminalAttackType.ATM)
                    _targetAtm.OnRescued?.Invoke();
            }

            PeopleEvents.OnCriminalDecreased?.Invoke();
            //Delayer.DoActionAfterDelay(this, 9f, () => gameObject.SetActive(false));
        }

        public void SetBelongedPhase(Phase phase) => BelongedPhase = phase;

        #region FIND FUNCTIONS
        private Civillian FindRandomCivillian()
        {
            if (BelongedPhase.ActiveCivillians == null || BelongedPhase.ActiveCivillians.Count == 0) return null;
            Civillian randomCivillian = BelongedPhase.ActiveCivillians[UnityEngine.Random.Range(0, BelongedPhase.ActiveCivillians.Count)];
            BelongedPhase.RemoveActiveCivillian(randomCivillian);
            return randomCivillian;
        }
        private Civillian FindClosestCivillian()
        {
            if (CharacterManager.CivilliansInScene == null || CharacterManager.CivilliansInScene.Count == 0) return null;

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
