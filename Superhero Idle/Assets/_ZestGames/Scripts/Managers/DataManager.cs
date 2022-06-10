using UnityEngine;

namespace ZestGames
{
    public class DataManager : MonoBehaviour
    {
        public bool DeleteAllData = false;

        public int TotalMoney { get; private set; }

        public void Init(GameManager gameManager)
        {
            LoadData();

            CollectableEvents.OnCollect += IncreaseTotalMoney;
        }

        private void OnDisable()
        {
            CollectableEvents.OnCollect -= IncreaseTotalMoney;

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

        private void IncreaseTotalMoney(int amount)
        {
            TotalMoney += amount;
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
