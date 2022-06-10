using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZestGames
{
    public class InitManager : MonoBehaviour
    {
        public bool UseSceneTransition = false;

        private void Awake()
        {
            if (!UseSceneTransition)
                SceneManager.LoadScene(LevelHandler.GetSceneBuildIndexToBeLoaded());
        }
    }
}
