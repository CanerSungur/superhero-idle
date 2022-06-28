using UnityEngine;
using TMPro;
using ZestGames;
using System;

namespace SuperheroIdle
{
    public class PhaseUnlocker : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private int requestedMoney = 1000;
        [SerializeField] private Phase phaseToBeUnlocked;
        public Phase PhaseToBeUnlocked => phaseToBeUnlocked;

        [Header("-- CANVAS SETUP --")]
        [SerializeField] private TextMeshProUGUI remainingMoneyText;
        [SerializeField] private Transform moneyTransform;

        private int _consumedMoney;
        public bool PlayerIsInArea { get; set; }
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < requestedMoney;
        public Transform MoneyTransform => moneyTransform;

        private void Init()
        {
            PlayerIsInArea = false;
            LoadConsumedMoney();
            remainingMoneyText.text = (requestedMoney - _consumedMoney).ToString();

            PhaseEvents.OnConsumeMoney += UpdateConsumedMoney;
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnApplicationQuit()
        {
            SaveConsumedMoney();
        }

        private void OnApplicationPause(bool pause)
        {
            SaveConsumedMoney();
        }

        private void OnDisable()
        {
            SaveConsumedMoney();
            
            PhaseEvents.OnConsumeMoney -= UpdateConsumedMoney;
        }

        #region LOAD-SAVE
        private void SaveConsumedMoney()
        {
            PlayerPrefs.SetInt($"Phase-{phaseToBeUnlocked.Number}-ConsumedMoney", _consumedMoney);
            PlayerPrefs.Save();
        }
        private void LoadConsumedMoney()
        {
            _consumedMoney = PlayerPrefs.GetInt($"Phase-{phaseToBeUnlocked.Number}-ConsumedMoney");

            if (_consumedMoney == requestedMoney)
                UnlockPhase();
        }
        #endregion

        private void UnlockPhase()
        {
            if (!phaseToBeUnlocked.gameObject.activeSelf)
                phaseToBeUnlocked.gameObject.SetActive(true);

            gameObject.SetActive(false);
        }
        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (requestedMoney - _consumedMoney).ToString();
        private void UpdateConsumedMoney(PhaseUnlocker phaseUnlocker, int amount)
        {
            if (phaseUnlocker != this && phaseUnlocker.PhaseToBeUnlocked == this.phaseToBeUnlocked)
                _consumedMoney += amount;

            UpdateRemainingMoneyText();
        }

        #region PUBLICS
        public void ConsumeMoney(int amount)
        {
            if (amount > (requestedMoney - _consumedMoney))
            {
                CollectableEvents.OnConsume?.Invoke(requestedMoney - _consumedMoney);
                PhaseEvents.OnConsumeMoney?.Invoke(this, requestedMoney - _consumedMoney);
                _consumedMoney = requestedMoney;
            }
            else
            {
                if (amount > DataManager.TotalMoney)
                {
                    _consumedMoney += DataManager.TotalMoney;
                    CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
                    PhaseEvents.OnConsumeMoney?.Invoke(this, DataManager.TotalMoney);
                }
                else
                {
                    CollectableEvents.OnConsume?.Invoke(amount);
                    PhaseEvents.OnConsumeMoney?.Invoke(this, amount);
                    _consumedMoney += amount;
                }
            }

            UpdateRemainingMoneyText();

            if (_consumedMoney == requestedMoney)
            {
                MoneyCanvas.Instance.StopSpendingMoney();
                UnlockPhase();
            }
        }
        #endregion
    }
}
