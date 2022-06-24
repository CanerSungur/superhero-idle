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

        #region SINGLETON
        public static MoneyCanvas Instance;
        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            MiddlePointRectTransform = transform.GetChild(0).GetComponent<RectTransform>();
        }
        #endregion

        public void SpawnCollectMoney()
        {
            CollectMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.CollectMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<CollectMoney>();
            money.Init(this);
        }
        public void SpawnSpendMoney()
        {
            SpendMoney money = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.SpendMoney, Vector3.zero, Quaternion.identity, transform).GetComponent<SpendMoney>();
            money.Init(this);
            
        }

        public void StartSpendingMoney()
        {
            _spendMoneyEnum = SpendMoney();
            StartCoroutine(_spendMoneyEnum);
        }
        public void StopSpendingMoney()
        {
            StopCoroutine(_spendMoneyEnum);
        }

        private IEnumerator SpendMoney()
        {
            while (true)
            {
                SpawnSpendMoney();
                yield return _waitforSpendMoneyDelay;
            }
        }
    }
}
