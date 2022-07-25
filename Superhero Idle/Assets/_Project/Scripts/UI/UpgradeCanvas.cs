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
        private CustomButton _emptySpaceButton; // will trigger close button.
        public static bool IsOpen { get; private set; }

        #region ANIMATION
        private Animator _animator;
        private readonly int _openID = Animator.StringToHash("Open");
        private readonly int _closeID = Animator.StringToHash("Close");
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
                _emptySpaceButton = transform.GetChild(1).GetComponent<CustomButton>();
            }

            Delayer.DoActionAfterDelay(this, 0.5f, UpdateTexts);

            IsOpen = false;
            _closeButton.onClick.AddListener(CloseCanvasClicked);
            _emptySpaceButton.onClick.AddListener(CloseCanvasClicked);

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

            UpgradeEvents.OnOpenUpgradeCanvas += EnableCanvas;
            UpgradeEvents.OnCloseUpgradeCanvas += DisableCanvas;
        }

        private void OnDisable()
        {
            _closeButton.onClick.RemoveListener(CloseCanvasClicked);
            _emptySpaceButton.onClick.RemoveListener(CloseCanvasClicked);

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

            UpgradeEvents.OnOpenUpgradeCanvas -= EnableCanvas;
            UpgradeEvents.OnCloseUpgradeCanvas -= DisableCanvas;
        }

        private void UpdateTexts()
        {
            movementSpeedLevelText.text = $"Level {DataManager.MovementSpeedLevel}";
            movementSpeedCostText.text = DataManager.MovementSpeedCost.ToString();

            changeSpeedLevelText.text = $"Level {DataManager.ChangeTimeLevel}";
            changeSpeedCostText.text = DataManager.ChangeTimeCost.ToString();

            fightSpeedLevelText.text = $"Level {DataManager.FightPowerLevel}";
            fightSpeedCostText.text = DataManager.FightPowerCost.ToString();

            incomeIncreaseLevelText.text = $"Level {DataManager.IncomeIncreaseLevel}";
            incomeIncreaseCostText.text = DataManager.IncomeIncreaseCost.ToString();

            costDecreaseLevelText.text = $"Level {DataManager.CostDecreaseLevel}";
            costDecreaseCostText.text = DataManager.CostDecreaseCost.ToString();

            CheckForMoneySufficiency();
        }
        private void CheckForMoneySufficiency()
        {
            movementSpeedUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.MovementSpeedCost;
            changeSpeedUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.ChangeTimeCost;
            fightSpeedUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.FightPowerCost;
            incomeIncreaseUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.IncomeIncreaseCost;
            costDecreaseUpgradeButton.interactable = DataManager.TotalMoney >= DataManager.CostDecreaseCost;
        }

        #region UPGRADE FUNCTIONS
        private void CloseCanvas() => UpgradeEvents.OnCloseUpgradeCanvas?.Invoke();
        private void UpgradeMovementSpeed() => UpgradeEvents.OnUpgradeMovementSpeed?.Invoke();
        private void UpgradeChangeSpeed() => UpgradeEvents.OnUpgradeChangeTime?.Invoke();
        private void UpgradeFightSpeed() => UpgradeEvents.OnUpgradeFightTime?.Invoke();
        private void UpgradeIncome() => UpgradeEvents.OnUpgradeIncome?.Invoke();
        private void UpgradeCostDecrease() => UpgradeEvents.OnUpgradeCostDecrease?.Invoke();
        #endregion

        #region CLICK TRIGGER FUNCTIONS
        private void CloseCanvasClicked()
        {
            _closeButton.interactable = _emptySpaceButton.interactable = false;
            _closeButton.TriggerClick(CloseCanvas);
        }
        private void MovementSpeedUpgradeClicked() => movementSpeedUpgradeButton.TriggerClick(UpgradeMovementSpeed);
        private void ChangeSpeedUpgradeClicked() => changeSpeedUpgradeButton.TriggerClick(UpgradeChangeSpeed);
        private void FightSpeedUpgradeClicked() => fightSpeedUpgradeButton.TriggerClick(UpgradeFightSpeed);
        private void IncomeUpgradeClicked() => incomeIncreaseUpgradeButton.TriggerClick(UpgradeIncome);
        private void CostDecreaseUpgradeClicked() => costDecreaseUpgradeButton.TriggerClick(UpgradeCostDecrease);
        #endregion

        #region ANIMATOR FUNCTIONS
        private void EnableCanvas()
        {
            AudioHandler.PlayAudio(Enums.AudioType.UpgradeMenu);
            _closeButton.interactable = _emptySpaceButton.interactable = true;
            _animator.SetTrigger(_openID);
            IsOpen = true;

            CheckForMoneySufficiency();
        }
        private void DisableCanvas()
        {
            AudioHandler.PlayAudio(Enums.AudioType.UpgradeMenu);
            _closeButton.interactable = _emptySpaceButton.interactable = false;
            _animator.SetTrigger(_closeID);
            IsOpen = false;
        }
        #endregion
    }
}
