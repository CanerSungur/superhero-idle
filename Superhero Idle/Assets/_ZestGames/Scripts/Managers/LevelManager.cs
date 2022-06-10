using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZestGames
{
    public class LevelManager : MonoBehaviour
    {
        private GameManager _gameManager;

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;

            GameEvents.OnChangeScene += ChangeScene;
        }

        private void OnDisable()
        {
            GameEvents.OnChangeScene -= ChangeScene;
        }

        private void ChangeScene(Enums.GameEnd gameEnd)
        {
            if (gameEnd == Enums.GameEnd.Success)
                IncreaseLevel();

            SceneManager.LoadScene(LevelHandler.GetSceneBuildIndexToBeLoaded());
        }

        private void IncreaseLevel() => LevelHandler.IncreaseLevel(this);
    }
}
