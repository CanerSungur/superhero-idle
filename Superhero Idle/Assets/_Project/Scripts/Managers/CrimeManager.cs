using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class CrimeManager : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private float maxCrimeDuration = 5f;
        [SerializeField] private int maxCrimeCount = 5;
        
        public static float MaxCrimeDuration { get; private set; }
        public static int MaxCrimeCount { get; private set; }

        public void Init(GameManager gameManager)
        {
            MaxCrimeDuration = maxCrimeDuration;
            MaxCrimeCount = maxCrimeCount;
        }
    }
}
