using UnityEngine;
using TMPro;
using ZestGames;

namespace SuperheroIdle
{
    public class PhaseUnlocker : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Phase phaseToBeUnlocked;
        public Phase PhaseToBeUnlocked => phaseToBeUnlocked;
        private Phase _belongedPhase = null;

        [Header("-- CANVAS SETUP --")]
        [SerializeField] private TextMeshProUGUI remainingMoneyText;
        [SerializeField] private Transform moneyTransform;
        [SerializeField] private TextMeshProUGUI phaseText;

        private readonly int _coreRequiredMoney = 1000;
        private int _requiredMoney;
        private int _consumedMoney;
        public bool PlayerIsInArea { get; set; }
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < _requiredMoney;
        public Transform MoneyTransform => moneyTransform;

        public void Init(Phase phase)
        {
            PlayerIsInArea = false;
            UpdateRequiredMoney();
            phaseText.text = $"PHASE {phaseToBeUnlocked.Number}";

            LoadConsumedMoney();

            PhaseEvents.OnConsumeMoney += UpdateConsumedMoney;
            PhaseEvents.OnUnlockPhase += HandleUnlockPhase;
            PlayerEvents.OnSetCurrentCostDecrease += UpdateRequiredMoney;
        }

        private void OnEnable()
        {
            _belongedPhase = GetComponentInParent<Phase>();
            _belongedPhase.AddPhase(this);
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
            if (_belongedPhase)
                _belongedPhase.RemovePhase(this);
            
            PhaseEvents.OnConsumeMoney -= UpdateConsumedMoney;
            PhaseEvents.OnUnlockPhase -= HandleUnlockPhase;
            PlayerEvents.OnSetCurrentCostDecrease -= UpdateRequiredMoney;

            SaveConsumedMoney();
        }

        private void UnlockPhase()
        {
            if (!phaseToBeUnlocked.gameObject.activeSelf)
                phaseToBeUnlocked.gameObject.SetActive(true);

            gameObject.SetActive(false);
            PhaseEvents.OnUnlockPhase?.Invoke(this, phaseToBeUnlocked);
        }
        private void HandleUnlockPhase(PhaseUnlocker phaseUnlocker, Phase phaseToBeUnlocked)
        {
            if (phaseUnlocker != this && phaseToBeUnlocked == this.phaseToBeUnlocked)
                gameObject.SetActive(false);
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
            UpdateRemainingMoneyText();

            if (_consumedMoney == _requiredMoney)
                UnlockPhase();

            Debug.Log(gameObject.name + " " + _consumedMoney);
        }
        #endregion

        #region UPDATE FUNCTIONS
        private void UpdateRequiredMoney()
        {
            //Debug.Log(gameObject.name + " " + (int)((_coreRequiredMoney * phaseToBeUnlocked.Number) * DataManager.CurrentCostDecrease));
            _requiredMoney = _coreRequiredMoney * phaseToBeUnlocked.Number;
            _requiredMoney -= (int)(_requiredMoney * DataManager.CurrentCostDecrease);

            UpdateRemainingMoneyText();
        }
        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (_requiredMoney - _consumedMoney).ToString();
        private void UpdateConsumedMoney(PhaseUnlocker phaseUnlocker, int amount)
        {
            if (phaseUnlocker != this && phaseUnlocker.PhaseToBeUnlocked == this.phaseToBeUnlocked)
                _consumedMoney += amount;

            UpdateRemainingMoneyText();
        }
        #endregion

        #region PUBLICS
        public void ConsumeMoney(int amount)
        {
            if (amount > (_requiredMoney - _consumedMoney))
            {
                CollectableEvents.OnConsume?.Invoke(_requiredMoney - _consumedMoney);
                PhaseEvents.OnConsumeMoney?.Invoke(this, _requiredMoney - _consumedMoney);
                _consumedMoney = _requiredMoney;
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

            if (_consumedMoney == _requiredMoney)
            {
                MoneyCanvas.Instance.StopSpendingMoney();
                UnlockPhase();
            }
        }
        #endregion
    }
}
