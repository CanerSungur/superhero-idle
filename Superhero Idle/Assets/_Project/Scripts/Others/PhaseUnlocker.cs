using UnityEngine;
using TMPro;
using ZestGames;

namespace SuperheroIdle
{
    public class PhaseUnlocker : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private int phaseNumber;
        [SerializeField] private int requestedMoney = 1000;
        [SerializeField] private Phase phaseToBeUnlocked;

        [Header("-- CANVAS SETUP --")]
        [SerializeField] private TextMeshProUGUI remainingMoneyText;

        private int _consumedMoney;
        public bool PlayerIsInArea { get; set; }
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < requestedMoney;

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
            PlayerPrefs.SetInt($"Phase-{phaseNumber}-ConsumedMoney", _consumedMoney);
            PlayerPrefs.Save();
        }
        private void LoadConsumedMoney()
        {
            _consumedMoney = PlayerPrefs.GetInt($"Phase-{phaseNumber}-ConsumedMoney");
        }
        #endregion

        private void UnlockPhase()
        {
            phaseToBeUnlocked.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (requestedMoney - _consumedMoney).ToString();

        #region PUBLICS
        public void ConsumeMoney(int amount)
        {
            _consumedMoney += amount;
            UpdateRemainingMoneyText();

            if (_consumedMoney == requestedMoney)
                UnlockPhase();
        }
        #endregion
    }
}
