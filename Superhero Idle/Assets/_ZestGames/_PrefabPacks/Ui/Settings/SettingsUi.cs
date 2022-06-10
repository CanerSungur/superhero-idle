using UnityEngine;
using UnityEngine.UI;

namespace ZestGames
{
    public class SettingsUi : MonoBehaviour
    {
        private Animator _animator;
        public Animator Animator => _animator == null ? _animator = GetComponent<Animator>() : _animator;

        [Header("-- REFERENCES --")]
        [SerializeField] private Image soundImage;
        [SerializeField] private Image vibrationImage;

        [Header("-- IMAGES SETUP --")]
        [SerializeField] private Sprite soundOnSprite;
        [SerializeField] private Sprite soundOffSprite;
        [SerializeField] private Sprite vibrationOnSprite;
        [SerializeField] private Sprite vibrationOffSprite;


        private Color _enabledColor = Color.white;
        private Color _disabledColor = Color.gray;
        private bool _menuIsOpen = false;

        private readonly int _openID = Animator.StringToHash("Open");
        private readonly int _closeID = Animator.StringToHash("Close");

        private void Awake()
        {
            _enabledColor.a = 1f;
            _disabledColor.a = .5f;

            soundImage.sprite = soundOnSprite;
            vibrationImage.sprite = vibrationOnSprite;
            soundImage.color = vibrationImage.color = _enabledColor;
        }

        #region Menu

        public void ToggleMenu()
        {
            if (_menuIsOpen)
                CloseMenu();
            else
                OpenMenu();
        }

        private void OpenMenu()
        {
            Animator.SetTrigger(_openID);
            _menuIsOpen = true;
        }

        private void CloseMenu()
        {
            Animator.SetTrigger(_closeID);
            _menuIsOpen = false;
        }

        #endregion

        #region Sound

        public void ToggleSound()
        {
            if (SettingsManager.SoundOn)
                CloseSound();
            else
                OpenSound();
        }

        private void OpenSound()
        {
            SettingsManager.SoundOn = true;
            soundImage.color = _enabledColor;
            soundImage.sprite = soundOnSprite;
        }

        private void CloseSound()
        {
            SettingsManager.SoundOn = false;
            soundImage.color = _disabledColor;
            soundImage.sprite = soundOffSprite;
        }

        #endregion

        #region Vibration

        public void ToggleVibration()
        {
            if (SettingsManager.VibrationOn)
                CloseVibration();
            else
                OpenVibration();
        }

        private void OpenVibration()
        {
            SettingsManager.VibrationOn = true;
            vibrationImage.color = _enabledColor;
            vibrationImage.sprite = vibrationOnSprite;
        }

        private void CloseVibration()
        {
            SettingsManager.VibrationOn = false;
            vibrationImage.color = _disabledColor;
            vibrationImage.sprite = vibrationOffSprite;
        }

        #endregion
    }
}
