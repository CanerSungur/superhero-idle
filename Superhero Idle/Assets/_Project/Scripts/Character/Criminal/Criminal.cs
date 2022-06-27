using UnityEngine;
using System;
using ZestGames;
using System.Collections;
using ZestCore.Utility;

namespace SuperheroIdle
{
    public class Criminal : CharacterBase
    {
        private Player _player;
        private CriminalCollision _collisionHandler;
        private CrimeDurationHandler _crimeDurationHandler;
        private CriminalEffectHandler _effectHandler;
        private CriminalAttackDecider _attackDecider;
        private Hammer _hammer;

        #region GETTERS
        public Player Player => _player;
        public CriminalAttackDecider AttackDecider => _attackDecider;
        #endregion

        #region EVENTS
        public Action OnAttack, OnDecideToAttack, OnGetThrown;
        public Action<Enums.CriminalAttackType> OnProceedAttack;
        public Action<PoliceCar> OnGetTakenToPoliceCar;
        public Action<bool> OnRunAway; // true is success, false is fail
        #endregion

        #region PROPERTIES
        public bool IsAttacking { get; private set; }
        public bool AttackStarted { get; private set; }
        public bool RunningAway { get; private set; }
        public bool GettingTakenToPoliceCar { get; private set; }
        public Transform LeftCarryPoint { get; private set; }
        public Transform RightCarryPoint { get; private set; }
        public PoliceCar ActivatedPoliceCar { get; private set; }
        public Phase BelongedPhase { get; private set; }
        #endregion

        [Header("-- SETUP --")]
        [SerializeField] private int value = 10;
        private int _currentValue, _runningAwayValue;
        private IEnumerator _disableAfterTimeEnum;

        private void OnEnable()
        {
            _runningAwayValue = (int)(value * 0.5f);
            _currentValue = value;

            RightCarryPoint = transform.GetChild(transform.childCount - 1);
            LeftCarryPoint = transform.GetChild(transform.childCount - 2);

            ActivatedPoliceCar = null;
            AttackStarted = RunningAway = GettingTakenToPoliceCar = false;

            Init();

            #region SCRIPT INITIALIZERS
            if (!_player)
            {
                _player = FindObjectOfType<Player>();
                _collisionHandler = GetComponent<CriminalCollision>();
                _attackDecider = GetComponent<CriminalAttackDecider>();
                _crimeDurationHandler = GetComponent<CrimeDurationHandler>();
                _effectHandler = GetComponent<CriminalEffectHandler>();
                _hammer = GetComponentInChildren<Hammer>();

            }
            _collisionHandler.Init(this);
            _attackDecider.Init(this);
            _crimeDurationHandler.Init(this);
            _effectHandler.Init(this);
            _hammer.Init(this);
            #endregion

            OnAttack += StartAttacking;
            OnDefeated += Defeated;
            OnRunAway += RunAway;
            OnGetTakenToPoliceCar += GetTakenToPoliceCar;
        }

        private void OnDisable()
        {
            if (BelongedPhase)
                BelongedPhase.RemoveActiveCriminal(this);
            CharacterManager.RemoveCriminalCommitingCrime(this);

            OnAttack -= StartAttacking;
            OnDefeated -= Defeated;
            OnRunAway -= RunAway;
            OnGetTakenToPoliceCar -= GetTakenToPoliceCar;
        }

        private void GetTakenToPoliceCar(PoliceCar ignoreThis) => GettingTakenToPoliceCar = true;
        private void Defeated()
        {
            IsAttacking = AttackStarted = RunningAway = GettingTakenToPoliceCar = false;
            CharacterManager.RemoveCriminalCommitingCrime(this);

            StartSpawningMoney(0.05f, _currentValue, transform.position);

            if (PoliceManager.GetNextFreePoliceCar() != null)
            {
                PoliceCar policeCar = PoliceManager.GetNextFreePoliceCar();
                ActivatedPoliceCar = policeCar;
                policeCar.StartTheCar(this);
            }
            else
            {
                Debug.Log("No available free police car.");
                ActivatedPoliceCar = null;
                DisableAfterSomeTime(5);
            }
        }
        private void StartAttacking() => AttackStarted = true;
        private void RunAway(bool success)
        {
            IsAttacking = AttackStarted = GettingTakenToPoliceCar = false;
            RunningAway = true;
            _currentValue = _runningAwayValue;

            if (success)
            {
                if (_attackDecider.AttackType == Enums.CriminalAttackType.Civillian)
                    _attackDecider.TargetCivillian.OnDefeated?.Invoke();
                else if (_attackDecider.AttackType == Enums.CriminalAttackType.ATM)
                    _attackDecider.TargetAtm.OnDefeated?.Invoke();

                PeopleEvents.OnCivillianDecreased?.Invoke();
            }
            else
            {
                if (_attackDecider.AttackType == Enums.CriminalAttackType.Civillian)
                    _attackDecider.TargetCivillian.OnRescued?.Invoke();
                else if (_attackDecider.AttackType == Enums.CriminalAttackType.ATM)
                    _attackDecider.TargetAtm.OnRescued?.Invoke();
            }

            PeopleEvents.OnCriminalDecreased?.Invoke();
            DisableAfterSomeTime(30f);
        }

        private void DisableAfterSomeTime(float time)
        {
            Delayer.DoActionAfterDelay(this, time, () => {
                if (ActivatedPoliceCar)
                {
                    ActivatedPoliceCar.ResetCar(this);
                    ActivatedPoliceCar = null;
                }
                gameObject.SetActive(false);
            }, out _disableAfterTimeEnum);
        }

        #region PUBLICS
        public void SetBelongedPhase(Phase phase) => BelongedPhase = phase;
        public void SetAttackState(bool isAttacking) => IsAttacking = isAttacking;
        #endregion

        #region MONEY SPAWN FUNCTIONS
        private void StartSpawningMoney(float delay, int count, Vector3 spawnPosition) => StartCoroutine(SpawnMoney(delay, count, spawnPosition));
        private IEnumerator SpawnMoney(float delay, int count, Vector3 spawnPosition)
        {
            int currentCount = 0;
            while (currentCount < count)
            {
                MoneyCanvas.Instance.SpawnCollectMoney();
                currentCount++;

                yield return new WaitForSeconds(delay);
            }
        }
        #endregion
    }
}
