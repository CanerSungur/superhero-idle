using UnityEngine;
using DG.Tweening;
using ZestGames;

namespace SuperheroIdle
{
    public class SpendMoney : MonoBehaviour
    {
        private MoneyCanvas _moneyCanvas;
        private RectTransform _canvasRect;
        private RectTransform _rectTransform;
        private Camera _camera;
        private Vector2 _currentPosition;
        private PhaseUnlocker _phaseUnlocker = null;
        private PhoneBooth _phoneBooth = null;
        private float _disableTime;

        //private Vector3 _defaultScale = Vector3.one;
        //private Vector3 _maxScale = new Vector3(1.6f, 1.6f, 1.6f);
        //private Quaternion _defaultRotation = Quaternion.identity;
        //private Quaternion _maxRotation = Quaternion.Euler(30f, 0f, -4f);

        public void Init(MoneyCanvas moneyCanvas, PhaseUnlocker phaseUnlocker)
        {
            if (!_moneyCanvas)
            {
                _moneyCanvas = moneyCanvas;
                _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
                _rectTransform = GetComponent<RectTransform>();
                _camera = Camera.main;
            }

            _phaseUnlocker = phaseUnlocker;
            _disableTime = Time.time + 0.9f;
            _currentPosition = Hud.MoneyAnchoredPosition;
            _rectTransform.anchoredPosition = _currentPosition;

            //_rectTransform.localScale = _defaultScale;
            //_rectTransform.rotation = _defaultRotation;

            //_rectTransform.DORotate(new Vector3(30f, 0f, -4f), 0.75f);
            //_rectTransform.DOScale(_maxScale, 0.75f);
            //_rectTransform.DOAnchorPos3D(GetWorldPointToScreenPoint(phaseUnlocker), 1f).OnComplete(() => {
            //    gameObject.SetActive(false);
            //});
        }

        public void Init(MoneyCanvas moneyCanvas, PhoneBooth phoneBooth)
        {
            if (!_moneyCanvas)
            {
                _moneyCanvas = moneyCanvas;
                _canvasRect = _moneyCanvas.GetComponent<RectTransform>();
                _rectTransform = GetComponent<RectTransform>();
                _camera = Camera.main;
            }

            _phoneBooth = phoneBooth;
            _disableTime = Time.time + 0.9f;
            _currentPosition = Hud.MoneyAnchoredPosition;
            _rectTransform.anchoredPosition = _currentPosition;
        }

        private void OnDisable()
        {
            _phaseUnlocker = null;
        }

        private void Update()
        {
            if (_phaseUnlocker)
            {
                Vector2 travel = GetWorldPointToScreenPoint(_phaseUnlocker.MoneyTransform) - _rectTransform.anchoredPosition;
                _rectTransform.Translate(travel * 10f * Time.deltaTime, _camera.transform);


                if (Vector2.Distance(_rectTransform.anchoredPosition, GetWorldPointToScreenPoint(_phaseUnlocker.MoneyTransform)) < 25f)
                    gameObject.SetActive(false);

                if (Time.time >= _disableTime)
                    gameObject.SetActive(false);
            }
            else if (_phoneBooth)
            {
                Vector2 travel = GetWorldPointToScreenPoint(_phoneBooth.MoneyImageTransform) - _rectTransform.anchoredPosition;
                _rectTransform.Translate(travel * 10f * Time.deltaTime, _camera.transform);


                if (Vector2.Distance(_rectTransform.anchoredPosition, GetWorldPointToScreenPoint(_phoneBooth.MoneyImageTransform)) < 25f)
                    gameObject.SetActive(false);

                if (Time.time >= _disableTime)
                    gameObject.SetActive(false);
            }
        }

        private Vector2 GetWorldPointToScreenPoint(Transform transform)
        {
            Vector2 viewportPosition = _camera.WorldToViewportPoint(transform.position);
            Vector2 phaseUnlockerScreenPosition = new Vector2(
               (viewportPosition.x * _canvasRect.sizeDelta.x) - (_canvasRect.sizeDelta.x * 1f),
               (viewportPosition.y * _canvasRect.sizeDelta.y) - (_canvasRect.sizeDelta.y * 1f));

            return phaseUnlockerScreenPosition;
        }
    }
}
