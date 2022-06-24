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

        private float _currentSpendMoneyDelay;
        private readonly float _startingSpendMoneyDelay = 0.25f;
        private readonly float _delayDecreaseRate = 0.05f;
        private readonly float _minDelay = 0.001f;
        private IEnumerator _spendMoneyEnum;

        private void Start()
        {
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            CharacterManager.SetPlayerTransform(transform);

            InputHandler.Init(this);
            CharacterController.Init(this);
            AnimationController.Init(this);
            CollisionController.Init(this);
            StateController.Init(this);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (StateController.CurrentState == Enums.PlayerState.Civillian)
                    PlayerEvents.OnChangeToHero?.Invoke();
                else if (StateController.CurrentState == Enums.PlayerState.Hero)
                    PlayerEvents.OnChangeToCivillian?.Invoke();

                //Money2D money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Money, transform.position, Quaternion.identity).GetComponent<Money2D>();
                //money.Collect(Vector3.zero);
            }

            if (Input.GetKey(KeyCode.S))
                MoneyCanvas.Instance.SpawnCollectMoney();
            if (Input.GetKeyDown(KeyCode.A))
            {
                MoneyCanvas.Instance.StartSpendingMoney();
            }
            if (Input.GetKeyDown(KeyCode.D))
                MoneyCanvas.Instance.StopSpendingMoney();
        }

        #region SPEND MONEY FUNCTIONS
        public void StartSpendingMoney(PhaseUnlocker phaseUnlocker)
        {
            _spendMoneyEnum = SpendMoney(phaseUnlocker);
            _currentSpendMoneyDelay = _startingSpendMoneyDelay;
            StartCoroutine(_spendMoneyEnum);
        }

        public void StopSpendingMoney(PhaseUnlocker phaseUnlocker)
        {
            StopCoroutine(_spendMoneyEnum);
        }

        private IEnumerator SpendMoney(PhaseUnlocker phaseUnlocker)
        {
            while (phaseUnlocker.MoneyCanBeSpent)
            {
                CollectableEvents.OnConsume?.Invoke(DataManager.MoneyValue);
                //Money2D money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Money, transform.position + Vector3.up, Quaternion.identity).GetComponent<Money2D>();
                //money.Spend(phaseUnlocker);

                yield return new WaitForSeconds(_currentSpendMoneyDelay);
                DecreaseMoneyDelay();
            }

        }
        private void DecreaseMoneyDelay()
        {
            _currentSpendMoneyDelay -= _delayDecreaseRate;
            if (_currentSpendMoneyDelay <= _minDelay)
                _currentSpendMoneyDelay = _minDelay;
        }
        #endregion
    }
}
