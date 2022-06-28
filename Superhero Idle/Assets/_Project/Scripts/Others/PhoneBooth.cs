using UnityEngine;
using ZestGames;
using TMPro;

namespace SuperheroIdle
{
    public class PhoneBooth : MonoBehaviour
    {
        [SerializeField] private int number = 1;
        private Animator _animator;
        private Collider _collider;

        [Header("-- APPEREANCE SETUP --")]
        [SerializeField] private Material activeMaterial;
        [SerializeField] private Material passivedMaterial;
        private SkinnedMeshRenderer[] _meshes;

        #region PROPERTIES
        public bool PlayerIsInArea { get; set; }
        public Vector3 EntryPosition { get; private set; }
        public bool DoorIsOpen { get; private set; }
        public bool IsActivated { get; private set; }
        public bool CanBeActivated => !IsActivated && DataManager.TotalMoney > 0;
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < _requiredMoney;
        public Transform MoneyImageTransform => moneyImageTransform;
        #endregion

        #region ANIMATION
        private readonly int _enterID = Animator.StringToHash("Enter");
        private readonly int _exitID = Animator.StringToHash("Exit");
        private readonly int _exitSuccessfullyID = Animator.StringToHash("ExitSuccessful");
        #endregion

        #region MONEY CONSUME
        [Header("-- MONEY CONSUME SETUP --")]
        [SerializeField] private GameObject moneyCanvas;
        [SerializeField] private Transform moneyImageTransform;
        [SerializeField] private TextMeshProUGUI remainingMoneyText;
        private int _requiredMoney = 100;
        private int _consumedMoney;
        #endregion

        private void Init()
        {
            if (!_animator)
            {
                _animator = GetComponent<Animator>();
                _collider = GetComponent<Collider>();
                _meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
            }
            
            EnableTrigger();
            EntryPosition = transform.GetChild(1).position;
            _consumedMoney = 0;
            DoorIsOpen = IsActivated = PlayerIsInArea = false;

            LoadData();
            CheckForActivation();
            UpdateRemainingMoneyText();

            PlayerEvents.OnEnterPhoneBooth += HeroEntered;
            PlayerEvents.OnExitPhoneBooth += HeroExit;
        }
        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            PlayerEvents.OnEnterPhoneBooth -= HeroEntered;
            PlayerEvents.OnExitPhoneBooth -= HeroExit;

            SaveData();
        }
        
        private void EnableTrigger() => _collider.isTrigger = true;
        private void DisableTrigger() => _collider.isTrigger = false;
        private void HeroEntered(PhoneBooth phoneBooth)
        {
            if (phoneBooth != this) return;
            _animator.SetTrigger(_enterID);
            DoorIsOpen = true;
        }
        private void HeroExit() => _animator.SetTrigger(_exitID);
        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (_requiredMoney - _consumedMoney).ToString();
        private void DeActivate()
        {
            IsActivated = false;
            moneyCanvas.SetActive(true);
            for (int i = 0; i < _meshes.Length; i++)
                _meshes[i].material = passivedMaterial;
        }
        #region PUBLICS
        public void HeroExitSuccessfully()
        {
            _animator.SetTrigger(_exitSuccessfullyID);
            DoorIsOpen = false;
        }
        public void Activate()
        {
            IsActivated = true;
            moneyCanvas.SetActive(false);
            for (int i = 0; i < _meshes.Length; i++)
                _meshes[i].material = activeMaterial;
        }
        public void ConsumeMoney(int amount)
        {
            if (amount > (_requiredMoney - _consumedMoney))
            {
                CollectableEvents.OnConsume?.Invoke(_requiredMoney - _consumedMoney);
                _consumedMoney = _requiredMoney;
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

            if (_consumedMoney == _requiredMoney)
            {
                MoneyCanvas.Instance.StopSpendingMoney();
                Activate();
            }
        }
        #endregion

        #region SAVE-LOAD FUNCTIONS
        private void SaveData()
        {
            PlayerPrefs.SetInt($"PhoneBooth-{number}", IsActivated == true ? 1 : 0);
            PlayerPrefs.SetInt($"PhoneBooth-{number}-ConsumedMoney", _consumedMoney);
            PlayerPrefs.Save();
        }
        private void LoadData()
        {
            IsActivated = PlayerPrefs.GetInt($"PhoneBooth-{number}") == 1;
            _consumedMoney = PlayerPrefs.GetInt($"PhoneBooth-{number}-ConsumedMoney");
        }
        private void CheckForActivation()
        {
            if (IsActivated)
                Activate();
            else
                DeActivate();
        }
        #endregion
    }
}
