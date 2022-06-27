using UnityEngine;

namespace SuperheroIdle
{
    public class CanvasTest : MonoBehaviour
    {
        private MoneyCanvas _moneyCanvas;
        private RectTransform _rectTransform;
        private RectTransform _canvasRect;
        [SerializeField] private PhaseUnlocker phaseUnlocker;
        [SerializeField] private Camera cam;

        public void Start()
        {
            _moneyCanvas = GetComponentInParent<MoneyCanvas>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
        }

        private void Update()
        {
            Vector2 viewportPosition = cam.WorldToViewportPoint(phaseUnlocker.transform.position);
            Vector2 phaseUnlockerScreenPosition = new Vector2(
               (viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 0.5f),
               (viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 0.5f));

            _rectTransform.anchoredPosition = phaseUnlockerScreenPosition;
        }
    }
}
