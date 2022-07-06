using UnityEngine;

namespace ZestGames
{
    public class SettingsManager : MonoBehaviour
    {
        public static bool SoundOn { get; set; }
        public static bool VibrationOn { get; set; }

        public void Init(GameManager gameManager)
        {
            SoundOn = VibrationOn = true;
        }
    }
}
