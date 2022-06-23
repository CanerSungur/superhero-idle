using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class CrimeManager : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private float maxCrimeDuration = 5f;
        public static float MaxCrimeDuration { get; private set; }

        public void Init(GameManager gameManager)
        {
            MaxCrimeDuration = maxCrimeDuration;
        }
    }
}
