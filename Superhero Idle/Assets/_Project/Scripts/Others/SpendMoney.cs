using UnityEngine;
using DG.Tweening;
using ZestGames;

namespace SuperheroIdle
{
    public class SpendMoney : MonoBehaviour
    {
        private MoneyCanvas _moneyCanvas;
        private RectTransform _rectTransform;

        public void Init(MoneyCanvas moneyCanvas)
        {
            if (!_moneyCanvas)
                _moneyCanvas = moneyCanvas;

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchoredPosition = Hud.MoneyAnchoredPosition;
            _rectTransform.DOAnchorPos(_moneyCanvas.MiddlePointRectTransform.anchoredPosition, 1f).OnComplete(() => {
                gameObject.SetActive(false);
            });
        }
    }
}
