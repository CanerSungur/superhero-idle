using System.Collections;
using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class MoneyCanvas : MonoBehaviour
    {
        public RectTransform MiddlePointRectTransform { get; private set; }
        private WaitForSeconds _waitforSpendMoneyDelay = new WaitForSeconds(0.1f);
        private IEnumerator _spendMoneyEnum;
        public bool SpendMoneyEnumIsPlaying { get; private set; }

        #region SINGLETON
        public static MoneyCanvas Instance;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            MiddlePointRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
            SpendMoneyEnumIsPlaying = false;
        }
        #endregion

        public void SpawnCollectMoney()
        {
            CollectMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.CollectMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<CollectMoney>();
            money.Init(this);
        }
        public void SpawnSpendMoney(PhaseUnlocker phaseUnlocker)
        {
            SpendMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.SpendMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<SpendMoney>();
            money.Init(this, phaseUnlocker);
        }
        public void SpawnSpendMoney(PhoneBooth phoneBooth)
        {
            SpendMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.SpendMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<SpendMoney>();
            money.Init(this, phoneBooth);
        }
        public void StartSpendingMoney(PhaseUnlocker phaseUnlocker)
        {
            SpendMoneyEnumIsPlaying = true;
            _spendMoneyEnum = SpendMoney(phaseUnlocker);
            StartCoroutine(_spendMoneyEnum);
        }
        public void StartSpendingMoney(PhoneBooth phoneBooth)
        {
            SpendMoneyEnumIsPlaying = true;
            _spendMoneyEnum = SpendMoney(phoneBooth);
            StartCoroutine(_spendMoneyEnum);
        }
        public void StopSpendingMoney()
        {
            if (SpendMoneyEnumIsPlaying)
            {
                StopCoroutine(_spendMoneyEnum);
                SpendMoneyEnumIsPlaying = false;
            }
        }

        private IEnumerator SpendMoney(PhaseUnlocker phaseUnlocker)
        {
            while (DataManager.TotalMoney > 0)
            {
                SpawnSpendMoney(phaseUnlocker);
                AudioEvents.OnPlaySpendMoney?.Invoke();
                yield return _waitforSpendMoneyDelay;
            }
        }
        private IEnumerator SpendMoney(PhoneBooth phoneBooth)
        {
            while (DataManager.TotalMoney > 0)
            {
                SpawnSpendMoney(phoneBooth);
                AudioEvents.OnPlaySpendMoney?.Invoke();
                yield return _waitforSpendMoneyDelay;
            }
        }
    }
}
