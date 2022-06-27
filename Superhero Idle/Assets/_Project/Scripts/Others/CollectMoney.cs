using UnityEngine;
using DG.Tweening;
using ZestGames;

namespace SuperheroIdle
{
    public class CollectMoney : MonoBehaviour
    {
        private MoneyCanvas _moneyCanvas;
        private RectTransform _rectTransform;

        public void Init(MoneyCanvas moneyCanvas)
        {
            if (!_moneyCanvas)
                _moneyCanvas = moneyCanvas;

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = _moneyCanvas.MiddlePointRectTransform.anchoredPosition;
            _rectTransform.DOAnchorPos(Hud.MoneyAnchoredPosition, 1f).OnComplete(() => {
                CollectableEvents.OnCollect?.Invoke(DataManager.MoneyValue);
                gameObject.SetActive(false);
            });
        }
    }
}
