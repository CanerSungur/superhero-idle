using UnityEngine;
using TMPro;

namespace ZestGames
{
    public class LevelSuccess : MonoBehaviour
    {
        private TextMeshProUGUI _levelText;
        private CustomButton _nextButton;

        public void Init(UiManager uiManager)
        {
            _levelText = transform.GetChild(transform.childCount - 1).GetComponentInChildren<TextMeshProUGUI>();
            _levelText.text = $"Level {LevelHandler.Level}";
            _nextButton = GetComponentInChildren<CustomButton>();
            _nextButton.onClick.AddListener(NextButtonClicked);
        }

        private void OnDisable()
        {
            _nextButton.onClick.RemoveListener(NextButtonClicked);
        }

        private void NextButtonClicked() => _nextButton.TriggerClick(() => GameEvents.OnChangeScene?.Invoke(GameManager.GameEnd));
    }
}
