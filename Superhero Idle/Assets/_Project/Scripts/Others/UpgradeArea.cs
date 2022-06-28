using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace SuperheroIdle
{
    public class UpgradeArea : MonoBehaviour
    {
        private UpgradeCanvas _upgradeCanvas;
        private Image _fillImage;
        private readonly float _openingTime = 3f;

        public bool PlayerIsInArea { get; private set; }

        private void Start()
        {
            _upgradeCanvas = FindObjectOfType<UpgradeCanvas>();
            _fillImage = transform.GetChild(0).GetChild(0).GetComponent<Image>();
            _fillImage.fillAmount = 0f;
            PlayerIsInArea = false;
        }

        private void OnDisable()
        {
            transform.DOKill();
        }

        private void OpenUpgradeCanvas() => _upgradeCanvas.OnEnableCanvas?.Invoke();
        private void CloseUpgradeCanvas() => _upgradeCanvas.OnDisableCanvas?.Invoke();

        #region PUBLICS
        public void StartOpening()
        {
            PlayerIsInArea = true;
            transform.DOPause();
            DOVirtual.Float(_fillImage.fillAmount, 1f, _openingTime, r => {
                _fillImage.fillAmount = r;
            }).OnComplete(OpenUpgradeCanvas);
        }
        public void StopOpening()
        {
            PlayerIsInArea = false;
            CloseUpgradeCanvas();
            transform.DOPause();
            DOVirtual.Float(_fillImage.fillAmount, 0f, _openingTime * 0.5f, r =>{
                _fillImage.fillAmount = r;
            });
        }
        #endregion
    }
}
