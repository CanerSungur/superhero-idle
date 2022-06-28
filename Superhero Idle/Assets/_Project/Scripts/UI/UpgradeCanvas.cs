using UnityEngine;
using TMPro;
using ZestCore.Utility;
using ZestGames;
using System;

namespace SuperheroIdle
{
    public class UpgradeCanvas : MonoBehaviour
    {
        private CustomButton _closeButton;

        #region ANIMATION
        private Animator _animator;
        private readonly int _openID = Animator.StringToHash("Open");
        private readonly int _closeID = Animator.StringToHash("Close");
        #endregion

        #region EVENTS
        public Action OnEnableCanvas, OnDisableCanvas;
        #endregion

        [Header("-- MOVEMENT SPEED SETUP --")]
        [SerializeField] private TextMeshProUGUI movementSpeedLevelText;
        [SerializeField] private TextMeshProUGUI movementSpeedCostText;
        [SerializeField] private CustomButton movementSpeedUpgradeButton;

        [Header("-- CHANGE SPEED SETUP --")]
        [SerializeField] private TextMeshProUGUI changeSpeedLevelText;
        [SerializeField] private TextMeshProUGUI changeSpeedCostText;
        [SerializeField] private CustomButton changeSpeedUpgradeButton;

        [Header("-- FIGHT SPEED SETUP --")]
        [SerializeField] private TextMeshProUGUI fightSpeedLevelText;
        [SerializeField] private TextMeshProUGUI fightSpeedCostText;
        [SerializeField] private CustomButton fightSpeedUpgradeButton;

        [Header("-- INCOME INCREASE SETUP --")]
        [SerializeField] private TextMeshProUGUI incomeIncreaseLevelText;
        [SerializeField] private TextMeshProUGUI incomeIncreaseCostText;
        [SerializeField] private CustomButton incomeIncreaseUpgradeButton;

        [Header("-- COST DECREASE SETUP --")]
        [SerializeField] private TextMeshProUGUI costDecreaseLevelText;
        [SerializeField] private TextMeshProUGUI costDecreaseCostText;
        [SerializeField] private CustomButton costDecreaseUpgradeButton;

        private void OnEnable()
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();
                _closeButton = transform.GetChild(0).GetChild(0).GetComponent<CustomButton>();
            }

            Delayer.DoActionAfterDelay(this, 0.5f, UpdateTexts);

            _closeButton.onClick.AddListener(CloseCanvasClicked);

            movementSpeedUpgradeButton.onClick.AddListener(MovementSpeedUpgradeClicked);
            changeSpeedUpgradeButton.onClick.AddListener(ChangeSpeedUpgradeClicked);
            fightSpeedUpgradeButton.onClick.AddListener(FightSpeedUpgradeClicked);
            incomeIncreaseUpgradeButton.onClick.AddListener(IncomeUpgradeClicked);
            costDecreaseUpgradeButton.onClick.AddListener(CostDecreaseUpgradeClicked);

            UiEvents.OnUpdateMovementSpeedText += UpdateTexts;
            UiEvents.OnUpdateChangeSpeedText += UpdateTexts;
            UiEvents.OnUpdateFightSpeedText += UpdateTexts;
            UiEvents.OnUpdateIncomeIncreaseText += UpdateTexts;
            UiEvents.OnUpdateCostDecreaseText += UpdateTexts;

            OnEnableCanvas += EnableCanvas;
            OnDisableCanvas += DisableCanvas;
        }

        private void OnDisable()
        {
            movementSpeedUpgradeButton.onClick.RemoveListener(MovementSpeedUpgradeClicked);
            changeSpeedUpgradeButton.onClick.RemoveListener(ChangeSpeedUpgradeClicked);
            fightSpeedUpgradeButton.onClick.RemoveListener(FightSpeedUpgradeClicked);
            incomeIncreaseUpgradeButton.onClick.RemoveListener(IncomeUpgradeClicked);
            costDecreaseUpgradeButton.onClick.RemoveListener(CostDecreaseUpgradeClicked);

            UiEvents.OnUpdateMovementSpeedText -= UpdateTexts;
            UiEvents.OnUpdateChangeSpeedText -= UpdateTexts;
            UiEvents.OnUpdateFightSpeedText -= UpdateTexts;
            UiEvents.OnUpdateIncomeIncreaseText -= UpdateTexts;
            UiEvents.OnUpdateCostDecreaseText -= UpdateTexts;

            OnEnableCanvas -= EnableCanvas;
            OnDisableCanvas -= DisableCanvas;
        }

        private void UpdateTexts()
        {
            movementSpeedLevelText.text = $"Level {DataManager.MovementSpeedLevel}";
            movementSpeedCostText.text = DataManager.MovementSpeedCost.ToString();

            changeSpeedLevelText.text = $"Level {DataManager.ChangeSpeedLevel}";
            changeSpeedCostText.text = DataManager.ChangeSpeedCost.ToString();

            fightSpeedLevelText.text = $"Level {DataManager.FightSpeedLevel}";
            fightSpeedCostText.text = DataManager.FightSpeedCost.ToString();

            incomeIncreaseLevelText.text = $"Level {DataManager.IncomeIncreaseLevel}";
            incomeIncreaseCostText.text = DataManager.IncomeIncreaseCost.ToString();

            costDecreaseLevelText.text = $"Level {DataManager.CostDecreaseLevel}";
            costDecreaseCostText.text = DataManager.CostDecreaseCost.ToString();

            CheckForMoneySufficiency();
        }
        private void CheckForMoneySufficiency()
        {
            movementSpeedUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.MovementSpeedCost;
            changeSpeedUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.ChangeSpeedCost;
            fightSpeedUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.FightSpeedCost;
            incomeIncreaseUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.IncomeIncreaseCost;
            costDecreaseUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.CostDecreaseCost;
        }

        #region UPGRADE FUNCTIONS
        private void UpgradeMovementSpeed() => UpgradeEvents.OnUpgradeMovementSpeed?.Invoke();
        private void UpgradeChangeSpeed() => UpgradeEvents.OnUpgradeChangeSpeed?.Invoke();
        private void UpgradeFightSpeed() => UpgradeEvents.OnUpgradeFightSpeed?.Invoke();
        private void UpgradeIncome() => UpgradeEvents.OnUpgradeIncome?.Invoke();
        private void UpgradeCostDecrease() => UpgradeEvents.OnUpgradeCostDecrease?.Invoke();
        #endregion

        #region CLICK TRIGGER FUNCTIONS
        private void CloseCanvasClicked() => _closeButton.TriggerClick(DisableCanvas);
        private void MovementSpeedUpgradeClicked() => movementSpeedUpgradeButton.TriggerClick(UpgradeMovementSpeed);
        private void ChangeSpeedUpgradeClicked() => changeSpeedUpgradeButton.TriggerClick(UpgradeChangeSpeed);
        private void FightSpeedUpgradeClicked() => fightSpeedUpgradeButton.TriggerClick(UpgradeFightSpeed);
        private void IncomeUpgradeClicked() => incomeIncreaseUpgradeButton.TriggerClick(UpgradeIncome);
        private void CostDecreaseUpgradeClicked() => costDecreaseUpgradeButton.TriggerClick(UpgradeCostDecrease);
        #endregion

        #region ANIMATOR FUNCTIONS
        private void EnableCanvas() => _animator.SetTrigger(_openID);
        private void DisableCanvas() => _animator.SetTrigger(_closeID);
        #endregion
    }
}
