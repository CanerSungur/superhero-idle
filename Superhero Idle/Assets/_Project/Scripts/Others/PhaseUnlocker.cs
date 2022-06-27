using UnityEngine;
using TMPro;
using ZestGames;

namespace SuperheroIdle
{
    public class PhaseUnlocker : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private int requestedMoney = 1000;
        [SerializeField] private Phase phaseToBeUnlocked;

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

        #region PUBLICS
        public void ConsumeMoney(int amount)
        {
            if (amount > (requestedMoney - _consumedMoney))
            {
                CollectableEvents.OnConsume?.Invoke(requestedMoney - _consumedMoney);
                _consumedMoney = requestedMoney;
            }
            else
            {
                if (amount > DataManager.TotalMoney)
                {
                    _consumedMoney += DataManager.TotalMoney;
                    CollectableEvents.OnConsume?.Invoke(DataManager.TotalMoney);
                }
                else
                {
                    CollectableEvents.OnConsume?.Invoke(amount);
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
