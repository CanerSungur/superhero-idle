using UnityEngine;
using ZestGames;
using ZestCore.Utility;

namespace SuperheroIdle
{
    public class Phase : MonoBehaviour
    {
        [Header("-- SPAWN SETUP --")]
        [SerializeField] private int maxCivillianCount = 50;
        [SerializeField] private int maxCriminalCount = 20;
        private Collider _collider;

        private void Init()
        {
            _collider = GetComponent<Collider>();

            for (int i = 0; i < maxCivillianCount; i++)
                SpawnCivillian();

            for (int i = 0; i < maxCriminalCount; i++)
                SpawnCriminal();

            PeopleEvents.OnCivillianDecreased += SpawnCivillian;
            PeopleEvents.OnCriminalDecreased += SpawnCriminal;
        }

        private void Start()
        {
            Delayer.DoActionAfterDelay(this, 2f, Init);
        }

        private void OnDisable()
        {
            PeopleEvents.OnCivillianDecreased -= SpawnCivillian;
            PeopleEvents.OnCriminalDecreased -= SpawnCriminal;
        }

        private void SpawnCivillian()
        {
            ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Civillian, RNG.RandomPointInBounds(_collider.bounds), Quaternion.identity);
        }
        private void SpawnCriminal()
        {
            ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Criminal, RNG.RandomPointInBounds(_collider.bounds), Quaternion.identity);
        }
    }
}
