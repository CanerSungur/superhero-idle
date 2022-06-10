using System.Collections.Generic;
using UnityEngine;

namespace ZestGames
{
    public class ObjectPooler : MonoBehaviour
    {
        // Default is this object. You can serialize this to be another container object.
        private Transform poolContainer;

        //public PoolData PoolData;
        public List<Pool> Pools;
        public Dictionary<Enums.PoolStamp, Queue<GameObject>> PoolDictionary;

        // Quick singleton to access easily.
        #region Singleton

        public static ObjectPooler Instance;

        private void Awake()
        {
            Instance = this;
            poolContainer = this.transform;
        }

        #endregion

        private void Start()
        {
            PoolDictionary = new Dictionary<Enums.PoolStamp, Queue<GameObject>>();

            foreach (Pool pool in Pools)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();

                for (int i = 0; i < pool.Size; i++)
                {
                    // If PoolData has just 1 object;
                    //GameObject obj = Instantiate(pool.PoolData.PoolObject, PoolContainer);

                    // PoolData can have more than 1 object.
                    //GameObject obj = Instantiate(pool.PoolData.PoolObjects[Random.Range(0, pool.PoolData.PoolObjects.Length)], PoolContainer);
                    //GameObject obj = Instantiate(PoolData.PoolObjects[Random.Range(0, PoolData.PoolObjects.Length)].Prefab, PoolContainer);
                    GameObject obj = Instantiate(pool.Prefab, poolContainer);

                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                PoolDictionary.Add(pool.PoolStamp, objectPool);
            }
        }

        public GameObject SpawnFromPool(Enums.PoolStamp poolStamp, Vector3 position, Quaternion rotation)
        {
            if (!PoolDictionary.ContainsKey(poolStamp))
            {
                Debug.LogError($"Pool with stamp: '{poolStamp}' doesn't exist.");
                return null;
            }

            // Pull out first element and store it
            GameObject objectToSpawn = PoolDictionary[poolStamp].Dequeue();

            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            // Add it back to our queue to use it later.
            PoolDictionary[poolStamp].Enqueue(objectToSpawn);

            return objectToSpawn;
        }
    }
}
