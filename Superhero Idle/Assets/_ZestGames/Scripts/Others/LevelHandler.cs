using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZestGames
{
    public static class LevelHandler
    {
        public static int Level { get; private set; }
        private static int _currentLevel, _lastSceneBuildIndex;

        public static int GetSceneBuildIndexToBeLoaded()
        {
            Level = PlayerPrefs.GetInt("Level", 1);

            // Uncomment this and run game once to reset level.
            //DeleteLevelData();

            _lastSceneBuildIndex = SceneManager.sceneCountInBuildSettings - 1;
            int index = Level % _lastSceneBuildIndex;
            if (index == 0)
                _currentLevel = _lastSceneBuildIndex;
            else
                _currentLevel = index;

            return _currentLevel;
        }

        public static void IncreaseLevel(LevelManager levelManager)
        {
            Level++;
            PlayerPrefs.SetInt("Level", Level);
            PlayerPrefs.Save();
        }
    }
}
