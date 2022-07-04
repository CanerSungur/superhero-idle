using UnityEngine;
using ZestGames;
using TMPro;
using DG.Tweening;

namespace SuperheroIdle
{
    public class PhoneBooth : MonoBehaviour
    {
        private Phase _belongedPhase;
        [SerializeField] private int number = 0;
        private Collider _collider;

        [Header("-- APPEREANCE SETUP --")]
        [SerializeField] private Material activeMaterial;
        [SerializeField] private Material passivedMaterial;
        private SkinnedMeshRenderer[] _meshes;

        #region SCRIPT REFERENCES
        private PhoneBoothAnimationController _animationController;
        public PhoneBoothAnimationController AnimationController => _animationController == null ? _animationController = GetComponent<PhoneBoothAnimationController>() : _animationController;
        #endregion

        #region PROPERTIES
        public bool PlayerIsInArea { get; set; }
        public Vector3 EntryPosition { get; private set; }
        public bool DoorIsOpen { get; private set; }
        public bool IsActivated { get; private set; }
        public bool CanBeActivated => !IsActivated && DataManager.TotalMoney > 0;
        public bool MoneyCanBeSpent => DataManager.TotalMoney > 0 && _consumedMoney < _requiredMoney;
        public Transform MoneyImageTransform => moneyImageTransform;
        #endregion
        
        #region MONEY CONSUME
        [Header("-- MONEY CONSUME SETUP --")]
        [SerializeField] private GameObject moneyCanvas;
        [SerializeField] private Transform moneyImageTransform;
        [SerializeField] private TextMeshProUGUI remainingMoneyText;
        private int _requiredMoney = 100;
        private readonly int _coreRequiredMoney = 100;
        private int _consumedMoney;
        #endregion

        private void Init()
        {
            if (!_collider)
            {
                _collider = GetComponent<Collider>();
                _meshes = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            AnimationController.Init(this);

            EnableTrigger();
            EntryPosition = transform.GetChild(1).position;
            _consumedMoney = 0;
            DoorIsOpen = IsActivated = PlayerIsInArea = false;
            
            UpdateRequiredMoney();
            LoadData();
            CheckForActivation();
            UpdateRemainingMoneyText();

            PlayerEvents.OnEnterPhoneBooth += HeroEntered;
            PlayerEvents.OnExitPhoneBoothSuccessfully += HeroExitSuccessfully;
            PlayerEvents.OnSetCurrentCostDecrease += UpdateRequiredMoney;
        }
        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            PlayerEvents.OnEnterPhoneBooth -= HeroEntered;
            PlayerEvents.OnExitPhoneBoothSuccessfully -= HeroExitSuccessfully;
            PlayerEvents.OnSetCurrentCostDecrease -= UpdateRequiredMoney;

            SaveData();
        }
        #region HELPERS
        private void Bounce()
        {
            transform.DORewind();
            transform.DOShakeScale(1f, 1f);
        }
        private void EnableTrigger() => _collider.isTrigger = true;
        private void DisableTrigger() => _collider.isTrigger = false;
        #endregion

        #region MONEY FUNCTIONS
        private void UpdateRequiredMoney()
        {
            if (!_belongedPhase)
                _belongedPhase = GetComponentInParent<Phase>();
            
            _requiredMoney = _coreRequiredMoney * _belongedPhase.Number;
            _requiredMoney -= (int)(_requiredMoney * DataManager.CurrentCostDecrease);
            UpdateRemainingMoneyText();
        }
        #endregion

        private void HeroEntered(PhoneBooth phoneBooth)
        {
            if (phoneBooth != this) return;
            DoorIsOpen = true;
        }
        private void UpdateRemainingMoneyText() => remainingMoneyText.text = (_requiredMoney - _consumedMoney).ToString();
        private void DeActivate()
        {
            IsActivated = false;
            moneyCanvas.SetActive(true);
            for (int i = 0; i < _meshes.Length; i++)
                _meshes[i].material = passivedMaterial;
        }
        private void HeroExitSuccessfully(PhoneBooth phoneBooth)
        {
            if (phoneBooth != this) return;
            DoorIsOpen = false;
        }
        private void Activate()
        {
            Bounce();
            IsActivated = true;
            moneyCanvas.SetActive(false);
            for (int i = 0; i < _meshes.Length; i++)
                _meshes[i].material = activeMaterial;
        }

        #region PUBLICS
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
