using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace SuperheroIdle
{
    public class PeopleSpawnManager : MonoBehaviour
    {
        [Header("-- SPAWN SETUP --")]
        [SerializeField] private int maxCivillianCount = 50;
        [SerializeField] private int maxCriminalCount = 20;
        [SerializeField] private Collider spawnAreaCollider;

        public void Init(GameManager gameManager)
        {
            for (int i = 0; i < maxCivillianCount; i++)
                SpawnCivillian();

            for (int i = 0; i < maxCriminalCount; i++)
                SpawnCriminal();

            PeopleEvents.OnCivillianDecreased += SpawnCivillian;
            PeopleEvents.OnCriminalDecreased += SpawnCriminal;
        }

        private void OnDisable()
        {
            PeopleEvents.OnCivillianDecreased -= SpawnCivillian;
            PeopleEvents.OnCriminalDecreased -= SpawnCriminal;
        }

        private void SpawnCivillian()
        {
            ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Civillian, RNG.RandomPointInBounds(spawnAreaCollider.bounds), Quaternion.identity);
        }
        private void SpawnCriminal()
        {
            ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Criminal, RNG.RandomPointInBounds(spawnAreaCollider.bounds), Quaternion.identity);
        }
    }
}
