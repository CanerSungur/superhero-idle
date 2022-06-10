using UnityEngine;
using ZestCore.Utility;

namespace ZestGames
{
    public class UiManager : MonoBehaviour
    {
        private GameManager _gameManager;

        [Header("-- REFERENCES --")]
        [SerializeField] private TouchToStart touchToStart;
        [SerializeField] private Hud hud;
        [SerializeField] private LevelFail levelFail;
        [SerializeField] private LevelSuccess levelSuccess;
        [SerializeField] private SettingsUi settings;

        [Header("-- UI DELAY SETUP --")]
        [SerializeField, Tooltip("The delay in seconds between the game is won and the win screen is loaded.")]
        private float successScreenDelay = 3.0f;
        [SerializeField, Tooltip("The delay in secods between the game is lost and the fail screen is loaded.")]
        private float failScreenDelay = 3.0f;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            hud.Init(this);
            levelFail.Init(this);
            levelSuccess.Init(this);

            touchToStart.gameObject.SetActive(true);
            hud.gameObject.SetActive(false);
            levelFail.gameObject.SetActive(false);
            levelSuccess.gameObject.SetActive(false);
            settings.gameObject.SetActive(false);

            GameEvents.OnGameStart += GameStarted;
            GameEvents.OnGameEnd += GameEnded;

            GameEvents.OnLevelSuccess += HandleLevelSuccess;
            GameEvents.OnLevelFail += HandleLevelFail;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= GameStarted;
            GameEvents.OnGameEnd -= GameEnded;

            GameEvents.OnLevelSuccess -= HandleLevelSuccess;
            GameEvents.OnLevelFail -= HandleLevelFail;
        }

        private void GameStarted()
        {
            touchToStart.gameObject.SetActive(false);
            hud.gameObject.SetActive(true);
            settings.gameObject.SetActive(true);
        }

        private void GameEnded(Enums.GameEnd gameEnd)
        {
            if (gameEnd == Enums.GameEnd.Fail)
                GameEvents.OnLevelFail?.Invoke();
            else if (gameEnd == Enums.GameEnd.Success)
                GameEvents.OnLevelSuccess?.Invoke();

            hud.gameObject.SetActive(false);
            settings.gameObject.SetActive(false);
        }

        private void HandleLevelSuccess()
        {
            Delayer.DoActionAfterDelay(this, successScreenDelay, () => levelSuccess.gameObject.SetActive(true));
        }

        private void HandleLevelFail()
        {
            Delayer.DoActionAfterDelay(this, failScreenDelay, () => levelFail.gameObject.SetActive(true));
        }
    }
}
