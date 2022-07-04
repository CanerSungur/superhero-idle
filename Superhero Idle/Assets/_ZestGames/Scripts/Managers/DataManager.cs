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
        public static float CurrentChangeTime { get; private set; }
        public static float CurrentFightPower { get; private set; }
        public static float CurrentIncome { get; private set; }
        public static float CurrentCostDecrease { get; private set; }
        #endregion

        #region LEVELS
        public static int MovementSpeedLevel { get; private set; }
        public static int ChangeTimeLevel { get; private set; }
        public static int FightPowerLevel { get; private set; }
        public static int IncomeIncreaseLevel { get; private set; }
        public static int CostDecreaseLevel { get; private set; }
        #endregion

        #region COST
        // \[ Price = BaseCost \times Multiplier ^{(\#\:Owned)} \]
        public static int MovementSpeedCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, MovementSpeedLevel));
        public static int ChangeTimeCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, ChangeTimeLevel));
        public static int FightPowerCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, FightPowerLevel));
        public static int IncomeIncreaseCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, IncomeIncreaseLevel));
        public static int CostDecreaseCost => (int)(_upgradeCost * Mathf.Pow(_upgradeCostIncreaseRate, CostDecreaseLevel));
        #endregion

        #region UPGRADE COST DATA
        private static readonly int _upgradeCost = 50;
        private static readonly float _upgradeCostIncreaseRate = 1.5f;
        #endregion

        #region CORE DATA
        private readonly float _coreMovementSpeed = 4f;
        private readonly float _coreChangeTime = 3f;
        private readonly float _coreFightPower = 0f;
        private readonly int _coreIncome = 1;
        private readonly float _coreCostDecrease = 0f;
        #endregion

        #region UPGRADE INCREMENT DATA
        private readonly float _movementSpeedIncrease = 0.2f;
        private readonly float _changeTimeDecrease = 0.1f;
        private readonly float _fightPowerIncrease = 0.2f;
        private readonly int _incomeIncrease = 1;
        private readonly float _costDecrease = 2f;
        #endregion

        #region SAVE-LOAD DATA VARIABLES
        private readonly string _totalMoneyKey = "TotalMoney";
        private readonly string _movementSpeedLevelKey = "MovementSpeedLevel";
        private readonly string _changeTimeLevelKey = "ChangeTimeLevel";
        private readonly string _fightPowerLevelKey = "FightPowerLevel";
        private readonly string _incomeIncreaseKey = "IncomeIncreaseLevel";
        private readonly string _costDecreaseKey = "CostDecreaseLevel";
        #endregion

        public void Init(GameManager gameManager)
        {
            MoneyValue = 1;

            LoadData();

            UpdateMovementSpeed();
            UpdateChangeTime();
            UpdateFightPower();
            UpdateIncome();
            UpdateCostDecrease();

            TotalMoney = 10000;

            UpgradeEvents.OnUpgradeMovementSpeed += MovementSpeedUpgrade;
            UpgradeEvents.OnUpgradeChangeTime += ChangeTimeUpgrade;
            UpgradeEvents.OnUpgradeFightTime += FightTimeUpgrade;
            UpgradeEvents.OnUpgradeIncome += IncomeIncreaseUpgrade;
            UpgradeEvents.OnUpgradeCostDecrease += CostDecreaseUpgrade;

            CollectableEvents.OnCollect += IncreaseTotalMoney;
            CollectableEvents.OnConsume += DecreaseTotalMoney;
        }

        private void OnDisable()
        {
            UpgradeEvents.OnUpgradeMovementSpeed -= MovementSpeedUpgrade;
            UpgradeEvents.OnUpgradeChangeTime -= ChangeTimeUpgrade;
            UpgradeEvents.OnUpgradeFightTime -= FightTimeUpgrade;
            UpgradeEvents.OnUpgradeIncome -= IncomeIncreaseUpgrade;
            UpgradeEvents.OnUpgradeCostDecrease -= CostDecreaseUpgrade;

            CollectableEvents.OnCollect -= IncreaseTotalMoney;
            CollectableEvents.OnConsume -= DecreaseTotalMoney;

            SaveData();
        }

        #region UPGRADE FUNCTIONS
        private void MovementSpeedUpgrade()
        {
            IncreaseMovementSpeedLevel();
            UpdateMovementSpeed();
        }
        private void ChangeTimeUpgrade()
        {
            IncreaseChangeSpeedLevel();
            UpdateChangeTime();
        }
        private void FightTimeUpgrade()
        {
            IncreaseFightSpeedLevel();
            UpdateFightPower();
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
            //Debug.Log("CurrentMovSpeed: " + CurrentMovementSpeed);
        }
        private void UpdateChangeTime()
        {
            CurrentChangeTime = _coreChangeTime - _changeTimeDecrease * (ChangeTimeLevel - 1);
            PlayerEvents.OnSetCurrentChangeTime?.Invoke();
            //Debug.Log("Change Time: " + CurrentChangeTime);
        }
        private void UpdateFightPower()
        {
            CurrentFightPower = _coreFightPower + _fightPowerIncrease * (FightPowerLevel - 1);
            PlayerEvents.OnSetCurrentFightPower?.Invoke();
            //Debug.Log("Fight Power: " + CurrentFightPower);
        }
        private void UpdateIncome()
        {
            MoneyValue = _coreIncome + _incomeIncrease * (IncomeIncreaseLevel - 1);
            //CurrentIncome = _coreIncome + _incomeIncrease * (IncomeIncreaseLevel - 1);
            PlayerEvents.OnSetCurrentIncomeIncrease?.Invoke();
            //Debug.Log("Money value: " + MoneyValue);
        }
        private void UpdateCostDecrease()
        {
            CurrentCostDecrease = (_coreCostDecrease + _costDecrease * (CostDecreaseLevel - 1)) / 100f;
            if (CurrentCostDecrease >= 0.6f)
                CurrentCostDecrease = 0.6f;
            PlayerEvents.OnSetCurrentCostDecrease?.Invoke();
            //Debug.Log("Cost Decrease Rate: " + CurrentCostDecrease);
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
            if (TotalMoney >= ChangeTimeCost)
            {
                TotalMoney -= ChangeTimeCost;
                ChangeTimeLevel++;
                UiEvents.OnUpdateChangeSpeedText?.Invoke();
                UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
            }
        }
        private void IncreaseFightSpeedLevel()
        {
            if (TotalMoney >= FightPowerCost)
            {
                TotalMoney -= FightPowerCost;
                FightPowerLevel++;
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
            ChangeTimeLevel = PlayerPrefs.GetInt(_changeTimeLevelKey, 1);
            FightPowerLevel = PlayerPrefs.GetInt(_fightPowerLevelKey, 1);
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
            PlayerPrefs.SetInt(_changeTimeLevelKey, ChangeTimeLevel);
            PlayerPrefs.SetInt(_fightPowerLevelKey, FightPowerLevel);
            PlayerPrefs.SetInt(_incomeIncreaseKey, IncomeIncreaseLevel);
            PlayerPrefs.SetInt(_costDecreaseKey, CostDecreaseLevel);

            PlayerPrefs.Save();
        }
        private void OnApplicationPause(bool pause) => SaveData();
        private void OnApplicationQuit() => SaveData();
        #endregion
    }
}
