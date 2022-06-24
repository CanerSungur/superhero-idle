using UnityEngine;

namespace ZestGames
{
    public class DataManager : MonoBehaviour
    {
        public bool DeleteAllData = false;

        public static int TotalMoney { get; private set; }
        public static int MoneyValue { get; private set; }

        public void Init(GameManager gameManager)
        {
            MoneyValue = 1;

            LoadData();

            CollectableEvents.OnCollect += IncreaseTotalMoney;
            CollectableEvents.OnConsume += DecreaseTotalMoney;
        }

        private void OnDisable()
        {
            CollectableEvents.OnCollect -= IncreaseTotalMoney;
            CollectableEvents.OnConsume -= DecreaseTotalMoney;

            SaveData();
        }

        private void OnApplicationPause(bool pause)
        {
            SaveData();
        }

        private void OnApplicationQuit()
        {
            SaveData();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M))
                CollectableEvents.OnCollect?.Invoke(1000);
#endif
        }

        private void IncreaseTotalMoney(int amount)
        {
            TotalMoney += amount;
            UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
        }

        private void DecreaseTotalMoney(int amount)
        {
            TotalMoney -= amount;
            UiEvents.OnUpdateCollectableText?.Invoke(TotalMoney);
        }

        private void LoadData()
        {
            TotalMoney = PlayerPrefs.GetInt("TotalMoney", 0);
        }

        private void SaveData()
        {
            if (DeleteAllData)
            {
                PlayerPrefs.DeleteAll();
                return;
            }

            PlayerPrefs.SetInt("TotalMoney", TotalMoney);
            PlayerPrefs.Save();
        }
    }
}
