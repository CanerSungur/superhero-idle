using UnityEngine;

namespace ZestGames
{
    public class SettingsManager : MonoBehaviour
    {
        private GameManager _gameManager;

        public static bool SoundOn { get; set; }
        public static bool VibrationOn { get; set; }

        public void Init(GameManager gameManager)
        {
            _gameManager = gameManager;
            SoundOn = VibrationOn = false;
        }
    }
}
