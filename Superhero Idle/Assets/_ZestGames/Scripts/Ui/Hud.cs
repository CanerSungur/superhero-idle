using UnityEngine;
using TMPro;
using ZestCore.Utility;

namespace ZestGames
{
    public class Hud : MonoBehaviour
    {
        [Header("-- TEXT --")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI collectableText;

        public static Transform CollectableHUDTransform { get; private set; }

        public void Init(UiManager uiManager)
        {
            UiEvents.OnUpdateLevelText += UpdateLevelText;
            UiEvents.OnUpdateCollectableText += UpdateMoneyText;

            CollectableHUDTransform = collectableText.transform.parent;
        }

        private void OnDisable()
        {
            UiEvents.OnUpdateLevelText -= UpdateLevelText;
            UiEvents.OnUpdateCollectableText -= UpdateMoneyText;
        }

        private void UpdateLevelText(int level) => levelText.text = $"Level {level}";
        private void UpdateMoneyText(int money)
        {
            collectableText.text = money.ToString();
            DOTweenUtils.ShakeTransform(transform, 0.25f);
        }
    }
}
