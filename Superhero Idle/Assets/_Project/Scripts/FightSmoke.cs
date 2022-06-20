using UnityEngine;
using System.Collections;
using ZestGames;

namespace SuperheroIdle
{
    public class FightSmoke : MonoBehaviour
    {
        private Criminal _criminal;

        [Header("-- SETUP --")]
        [SerializeField] private GameObject armObj;
        [SerializeField] private GameObject legObj;
        [SerializeField] private GameObject capeObj;

        private IEnumerator _spawnArmEnum, _spawnLegEnum, _spawnCapeEnum;
        private WaitForSeconds _waitForArmSpawnDelay, _waitForLegSpawnDelay, _waitForCapeSpawnDelay;

        public void Init(Criminal criminal)
        {
            _criminal = criminal;
            _waitForArmSpawnDelay = new WaitForSeconds(0.5f);
            _waitForLegSpawnDelay = new WaitForSeconds(0.75f);
            _waitForCapeSpawnDelay = new WaitForSeconds(1f);

            _spawnArmEnum = SpawnArm();
            _spawnLegEnum = SpawnLeg();
            //_spawnCapeEnum = SpawnCape();

            PlayerEvents.OnStartFighting += StartSpawning;
            PlayerEvents.OnStopFighting += StopSpawning;
        }

        private void OnDisable()
        {
            PlayerEvents.OnStartFighting -= StartSpawning;
            PlayerEvents.OnStopFighting -= StopSpawning;
        }

        private void StartSpawning(Criminal criminal)
        {
            if (criminal != _criminal) return;

            capeObj.SetActive(true);
            StartCoroutine(_spawnArmEnum);
            StartCoroutine(_spawnLegEnum);
            //StartCoroutine(_spawnCapeEnum);
        }
        private void StopSpawning(Criminal criminal)
        {
            if (criminal != _criminal) return;

            capeObj.SetActive(false);
            StopCoroutine(_spawnArmEnum);
            StopCoroutine(_spawnLegEnum);
            //StopCoroutine(_spawnCapeEnum);

            _spawnArmEnum = SpawnArm();
            _spawnLegEnum = SpawnLeg();
            //_spawnCapeEnum = SpawnCape();
        }

        #region SPAWNING FUNCTIONS
        private IEnumerator SpawnArm()
        {
            while (true)
            {
                var randomRot = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f);
                Instantiate(armObj, transform.position, randomRot);
                yield return _waitForArmSpawnDelay;
            }
        }
        private IEnumerator SpawnLeg()
        {
            while (true)
            {
                var randomRot = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f);
                Instantiate(legObj, transform.position, randomRot);
                yield return _waitForLegSpawnDelay;
            }
        }
        //private IEnumerator SpawnCape()
        //{
        //    while (true)
        //    {
        //        //var randomRot = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f);
        //        Instantiate(capeObj, transform.position, Quaternion.identity);
        //        yield return _waitForCapeSpawnDelay;
        //    }
        //}
        #endregion
    }
}
