using System.Collections;
using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class Player : MonoBehaviour
    {
        #region COMPONENT REFERENCES
        private Collider _collider;
        public Collider Collider => _collider == null ? _collider = GetComponent<Collider>() : _collider;
        #endregion

        #region SCRIPT REFERENCES
        private JoystickInput _inputHandler;
        public JoystickInput InputHandler => _inputHandler == null ? _inputHandler = GetComponent<JoystickInput>() : _inputHandler;
        private PlayerCharacterController _characterController;
        public PlayerCharacterController CharacterController => _characterController == null ? _characterController = GetComponent<PlayerCharacterController>() : _characterController;
        private PlayerAnimationController _animationController;
        public PlayerAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<PlayerAnimationController>() : _animationController;
        private PlayerCollision _collisionController;
        public PlayerCollision CollisionController => _collisionController == null ? _collisionController = GetComponent<PlayerCollision>() : _collisionController;
        private PlayerStateController _stateController;
        public PlayerStateController StateController => _stateController == null ? _stateController = GetComponent<PlayerStateController>() : _stateController;
        #endregion

        #region SPEND MONEY SECTION
        private float _currentSpendMoneyDelay;
        private readonly float _startingSpendMoneyDelay = 0.25f;
        private readonly float _delayDecreaseRate = 0.05f;
        private readonly float _minDelay = 0.001f;
        private IEnumerator _spendMoneyEnum;

        // Spend value will increase by 10 in every 5 spend counts to shorten spending time immensely.
        private int _currentMoneySpendValue, _moneySpendingCount;
        //private readonly int _moneyValueIncrementLimit = 20; 
        private readonly int _moneyValueMultiplier = 5;
        #endregion

        #region CRIMINAL IN SIGHT SECTION
        public bool CrimeHappeningNearby => ClosestActiveCriminal && _closestActiveCriminalDistance <= _nearbyCrimeDistanceLimit;
        public Criminal ClosestActiveCriminal { get; private set; }
        private float _closestActiveCriminalDistance;
        private readonly float _nearbyCrimeDistanceLimit = 64f;
        #endregion

        private void Start()
        {
            ClosestActiveCriminal = null;
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
            CharacterManager.SetPlayerTransform(transform);

            InputHandler.Init(this);
            CharacterController.Init(this);
            AnimationController.Init(this);
            CollisionController.Init(this);
            StateController.Init(this);
        }

        private void Update()
        {
            GetClosestActiveCriminal();
        }

        #region SPEND MONEY FUNCTIONS
        public void StartSpendingMoney(PhaseUnlocker phaseUnlocker)
        {
            _spendMoneyEnum = SpendMoney(phaseUnlocker);
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
            StartCoroutine(_spendMoneyEnum);

            // Start throwing money
            if (phaseUnlocker.MoneyCanBeSpent)
                MoneyCanvas.Instance.StartSpendingMoney(phaseUnlocker);
        }
        public void StartSpendingMoney(PhoneBooth phoneBooth)
        {
            _spendMoneyEnum = SpendMoney(phoneBooth);
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            _currentMoneySpendValue = DataManager.MoneyValue;
            _moneySpendingCount = 0;
            StartCoroutine(_spendMoneyEnum);

            // Start throwing money
            if (phoneBooth.MoneyCanBeSpent)
                MoneyCanvas.Instance.StartSpendingMoney(phoneBooth);
        }
        public void StopSpendingMoney(PhaseUnlocker phaseUnlocker)
        {
            StopCoroutine(_spendMoneyEnum);

            // Stop throwing money
            MoneyCanvas.Instance.StopSpendingMoney();
        }
        public void StopSpendingMoney(PhoneBooth phoneBooth)
        {
            StopCoroutine(_spendMoneyEnum);

            // Stop throwing money
            MoneyCanvas.Instance.StopSpendingMoney();
        }
        private IEnumerator SpendMoney(PhaseUnlocker phaseUnlocker)
        {
            while (phaseUnlocker.MoneyCanBeSpent)
            {
                //Debug.Log("Consumed Money: " + _currentMoneySpendValue);
                phaseUnlocker.ConsumeMoney(_currentMoneySpendValue);
                yield return new WaitForSeconds(_currentSpendMoneyDelay);
                //DecreaseMoneyDelay();
                UpdateMoneyValue();
            }
        }
        private IEnumerator SpendMoney(PhoneBooth phoneBooth)
        {
            while (phoneBooth.MoneyCanBeSpent)
            {
                //Debug.Log("Consumed Money: " + _currentMoneySpendValue);
                phoneBooth.ConsumeMoney(_currentMoneySpendValue);
                yield return new WaitForSeconds(_currentSpendMoneyDelay);
                //DecreaseMoneyDelay();
                UpdateMoneyValue();
            }
        }
        private void DecreaseMoneyDelay()
        {
            _currentSpendMoneyDelay -= _delayDecreaseRate;
            if (_currentSpendMoneyDelay <= _minDelay)
                _currentSpendMoneyDelay = _minDelay;
        }
        private void UpdateMoneyValue()
        {
            _moneySpendingCount++;
            if (_moneySpendingCount != 0 && _moneySpendingCount % 5 == 0)
            {
                _currentMoneySpendValue *= _moneyValueMultiplier;
                //Debug.Log(_currentMoneySpendValue);
            }
        }
        #endregion

        #region CRIMINAL SEARCH FUNCTIONS
        private void GetClosestActiveCriminal()
        {
            if (CharacterManager.CriminalsCommitingCrime == null || CharacterManager.CriminalsCommitingCrime.Count == 0)
            {
                ClosestActiveCriminal = null;
                _closestActiveCriminalDistance = float.MaxValue;
            }
            else
            {
                _closestActiveCriminalDistance = float.MaxValue;
                for (int i = 0; i < CharacterManager.CriminalsCommitingCrime.Count; i++)
                {
                    float distanceToTransform = (transform.position - CharacterManager.CriminalsCommitingCrime[i].transform.position).sqrMagnitude;
                    if (distanceToTransform < _closestActiveCriminalDistance && transform != CharacterManager.CriminalsCommitingCrime[i])
                    {
                        _closestActiveCriminalDistance = distanceToTransform;
                        ClosestActiveCriminal = CharacterManager.CriminalsCommitingCrime[i];
                    }
                }
            }
        }
        #endregion
    }
}
