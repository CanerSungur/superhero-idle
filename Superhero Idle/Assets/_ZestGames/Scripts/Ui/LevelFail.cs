using UnityEngine;

namespace ZestGames
{
    public class LevelFail : MonoBehaviour
    {
        private CustomButton _restartButton;

        public void Init(UiManager uiManager)
        {
            _restartButton = GetComponentInChildren<CustomButton>();
            _restartButton.onClick.AddListener(RestartButtonClicked);
        }

        private void OnDisable()
        {
            _restartButton.onClick.RemoveListener(RestartButtonClicked);
        }

        private void RestartButtonClicked() => _restartButton.TriggerClick(() => GameEvents.OnChangeScene?.Invoke(GameManager.GameEnd));
    }
}
