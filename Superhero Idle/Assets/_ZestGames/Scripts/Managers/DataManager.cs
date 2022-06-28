using UnityEngine;

namespace ZestGames
{
    public class DataManager : MonoBehaviour
    {
        public bool DeleteAllData = false;

        #region VALUES
        public static int TotalMoney { get; private set; }
        public static int MoneyValue { get; private set; }
        public static float CurrentMovementSpeed { get; private set; }
        public static float CurrentChangeSpeed { get; private set; }
        public static float CurrentFightSpeed { get; private set; }
        public static float CurrentIncome { get; private set; }
        public static float CurrentCostDecrease { get; private set; }
        #endregion

        #region LEVELS
        public static int MovementSpeedLevel { get; private set; }
        public static int ChangeSpeedLevel { get; private set; }
        public static int FightSpeedLevel { get; private set; }
        public static int IncomeIncreaseLevel { get; private set; }
        public static int CostDecreaseLevel { get; private set; }
        #endregion

        #region COST
        // \[ Price = BaseCost \times Multiplier ^{(\#\:Owned)} \]
        public static int MovementSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, MovementSpeedLevel));
        public static int ChangeSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, ChangeSpeedLevel));
        public static int FightSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, FightSpeedLevel));
        public static int IncomeIncreaseCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, IncomeIncreaseLevel));
        public static int CostDecreaseCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, CostDecreaseLevel));
        #endregion

        #region UPGRADE COST DATA
        private static readonly int _upgradeCost = 20;
        private static readonly float _upgradeCostIncreaseRate = 1.2f;
        #endregion

        #region CORE DATA
        private readonly float _coreMovementSpeed;
        private readonly float _coreChangeSpeed;
        private readonly float _coreFightSpeed;
        private readonly float _coreIncome;
        private readonly float _coreCostDecrease;
        #endregion

        #region UPGRADE INCREMENT DATA
        private readonly float _movementSpeedIncrease = 0.3f;
        private readonly float _changeSpeedIncrease = 0.1f;
        private readonly float _fightSpeedIncrease = 0.2f;
        private readonly int _incomeIncrease = 2;
        private readonly int _costDecrease = 10;
        #endregion

        #region SAVE-LOAD DATA VARIABLES
        private readonly string _totalMoneyKey = "TotalMoney";
        private readonly string _movementSpeedLevelKey = "MovementSpeedLevel";
        private readonly string _changeSpeedLevelKey = "ChangeSpeedLevel";
        private readonly string _fightSpeedLevelKey = "FightSpeedLevel";
        private readonly string _incomeIncreaseKey = "IncomeIncreaseLevel";
        private readonly string _costDecreaseKey = "CostDecreaseLevel";
        #endregion

        public void Init(GameManager gameManager)
        {
            MoneyValue = 1;

            LoadData();

            UpdateMovementSpeed();
            UpdateChangeSpeed();
            UpdateFightSpeed();
            UpdateIncome();
            UpdateCostDecrease();

            UpgradeEvents.OnUpgradeMovementSpeed += MovementSpeedUpgrade;
            UpgradeEvents.OnUpgradeChangeSpeed += ChangeSpeedUpgrade;
            UpgradeEvents.OnUpgradeFightSpeed += FightSpeedUpgrade;
            UpgradeEvents.OnUpgradeIncome += IncomeIncreaseUpgrade;
            UpgradeEvents.OnUpgradeCostDecrease += CostDecreaseUpgrade;

            CollectableEvents.OnCollect += IncreaseTotalMoney;
            CollectableEvents.OnConsume += DecreaseTotalMoney;
        }

        private void OnDisable()
        {
            UpgradeEvents.OnUpgradeMovementSpeed -= MovementSpeedUpgrade;
            UpgradeEvents.OnUpgradeChangeSpeed -= ChangeSpeedUpgrade;
            UpgradeEvents.OnUpgradeFightSpeed -= FightSpeedUpgrade;
            UpgradeEvents.OnUpgradeIncome -= IncomeIncreaseUpgrade;
            UpgradeEvents.OnUpgradeCostDecrease -= CostDecreaseUpgrade;

            CollectableEvents.OnCollect -= IncreaseTotalMoney;
            CollectableEvents.OnConsume -= DecreaseTotalMoney;

            SaveData();
        }

        #region FOR TESTING
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M))
                CollectableEvents.OnCollect?.Invoke(1000);
#endif
        }
        #endregion

        #region UPGRADE FUNCTIONS
        private void MovementSpeedUpgrade()
        {
            IncreaseMovementSpeedLevel();
            UpdateMovementSpeed();
        }
        private void ChangeSpeedUpgrade()
        {
            IncreaseChangeSpeedLevel();
            UpdateChangeSpeed();
        }
        private void FightSpeedUpgrade()
        {
            IncreaseFightSpeedLevel();
            UpdateFightSpeed();
        }
        private void IncomeIncreaseUpgrade()
        {
            IncreaseIncomeLevel();
            UpdateIncome();
        }
        private void CostDecreaseUpgrade()
        {
            IncreaseCostDecreaseLevel();
            UpdateCostDecrease();
        }
        #endregion

        #region UPDATE FUNCTIONS
        private void UpdateMovementSpeed()
        {
            CurrentMovementSpeed = _coreMovementSpeed + _movementSpeedIncrease * (MovementSpeedLevel - 1);
            PlayerEvents.OnSetCurrentMovementSpeed?.Invoke();

            //MaxStamina = _coreStamina + _staminaIncreaseRate * (StaminaForCurrentLevel - 1);
            //PlayerEvents.OnSetCurrentStamina?.Invoke();
            //Debug.Log("Stamina For CurrentLevel: " + StaminaForCurrentLevel);
            //Debug.Log("Max Stamina: " + MaxStamina);
        }
        private void UpdateChangeSpeed()
        {
            CurrentChangeSpeed = _coreChangeSpeed + _changeSpeedIncrease * (ChangeSpeedLevel - 1);
            PlayerEvents.OnSetCurrentChangeSpeed?.Invoke();
        }
        private void UpdateFightSpeed()
        {
            CurrentFightSpeed = _coreFightSpeed + _fightSpeedIncrease * (FightSpeedLevel - 1);
            PlayerEvents.OnSetCurrentFightSpeed?.Invoke();
        }
        private void UpdateIncome()
        {
            CurrentIncome = _coreIncome + _incomeIncrease * (IncomeIncreaseLevel - 1);
            PlayerEvents.OnSetCurrentIncomeIncrease?.Invoke();
        }
        private void UpdateCostDecrease()
        {
            CurrentCostDecrease = _coreCostDecrease + _costDecrease * (CostDecreaseLevel - 1);
            PlayerEvents.OnSetCurrentCostDecrease?.Invoke();
        }
        #endregion

        #region INCREMENT FUNCTIONS
        private void IncreaseMovementSpeedLevel()
        {
            if (TotalMoney >= MovementSpeedCost)
            {
                TotalMoney -= MovementSpeedCost;
                MovementSpeedLevel++;
                UiEvents.OnUpdateMovementSpeedText?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreaseChangeSpeedLevel()
        {
            if (TotalMoney >= ChangeSpeedCost)
            {
                TotalMoney -= ChangeSpeedCost;
                ChangeSpeedLevel++;
                UiEvents.OnUpdateChangeSpeedText?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreaseFightSpeedLevel()
        {
            if (TotalMoney >= FightSpeedCost)
            {
                TotalMoney -= FightSpeedCost;
                FightSpeedLevel++;
                UiEvents.OnUpdateFightSpeedText?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreaseIncomeLevel()
        {
            if (TotalMoney >= IncomeIncreaseCost)
            {
                TotalMoney -= IncomeIncreaseCost;
                IncomeIncreaseLevel++;
                UiEvents.OnUpdateIncomeIncreaseText?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreaseCostDecreaseLevel()
        {
            if (TotalMoney >= CostDecreaseCost)
            {
                TotalMoney -= CostDecreaseCost;
                CostDecreaseLevel++;
                UiEvents.OnUpdateCostDecreaseText?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        #endregion

        private void IncreaseTotalMoney(int amount)
        {
            TotalMoney += amount;
            UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
        }

        private void DecreaseTotalMoney(int amount)
        {
            TotalMoney -= amount;
            UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
        }

        #region SAVE-LOAD FUNCTIONS
        private void LoadData()
        {
            TotalMoney = PlayerPrefs.GetInt(_totalMoneyKey, 0);
            MovementSpeedLevel = PlayerPrefs.GetInt(_movementSpeedLevelKey, 1);
            ChangeSpeedLevel = PlayerPrefs.GetInt(_changeSpeedLevelKey, 1);
            FightSpeedLevel = PlayerPrefs.GetInt(_fightSpeedLevelKey, 1);
            IncomeIncreaseLevel = PlayerPrefs.GetInt(_incomeIncreaseKey, 1);
            CostDecreaseLevel = PlayerPrefs.GetInt(_costDecreaseKey, 1);
        }

        private void SaveData()
        {
            if (DeleteAllData)
            {
                PlayerPrefs.DeleteAll();
                return;
            }

            PlayerPrefs.SetInt(_totalMoneyKey, TotalMoney);
            PlayerPrefs.SetInt(_movementSpeedLevelKey, MovementSpeedLevel);
            PlayerPrefs.SetInt(_changeSpeedLevelKey, ChangeSpeedLevel);
            PlayerPrefs.SetInt(_fightSpeedLevelKey, FightSpeedLevel);
            PlayerPrefs.SetInt(_incomeIncreaseKey, IncomeIncreaseLevel);
            PlayerPrefs.SetInt(_costDecreaseKey, CostDecreaseLevel);

            PlayerPrefs.Save();
        }
        private void OnApplicationPause(bool pause) => SaveData();
        private void OnApplicationQuit() => SaveData();
        #endregion
    }
}
