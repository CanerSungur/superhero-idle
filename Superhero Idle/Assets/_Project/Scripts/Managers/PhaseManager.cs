using System.Collections.Generic;
using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class PhaseManager : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private Phase[] phases;
        //private int _unlockedPhaseNumber;
        //public int UnlockedPhaseNumber => _unlockedPhaseNumber;

        private List<int> _unlockedPhaseNumbers = new List<int>();
        public List<int> UnlockedPhaseNumbers => _unlockedPhaseNumbers;

        public void Init(GameManager gameManager)
        {
            _unlockedPhaseNumbers.Clear();
            _unlockedPhaseNumbers.Add(1);
            Load();
            UpdatePhaseActivation();

            PhaseEvents.OnUnlockPhase += PhaseUnlocked;
        }

        private void OnApplicationPause(bool pause)
        {
            Save();
        }
        private void OnApplicationQuit()
        {
            Save();
        }

        private void OnDisable()
        {
            PhaseEvents.OnUnlockPhase -= PhaseUnlocked;
            Save();
        }

        private void PhaseUnlocked(PhaseUnlocker phaseUnlocker, Phase phase)
        {
            if (!_unlockedPhaseNumbers.Contains(phase.Number))
                _unlockedPhaseNumbers.Add(phase.Number);

            phase.gameObject.SetActive(true);
            phase.Init(this);

            //_unlockedPhaseNumber++;
        }
        private void UpdatePhaseActivation()
        {
            for (int i = 0; i < phases.Length; i++)
            {
                Phase phase = phases[i];
                if (_unlockedPhaseNumbers.Contains(phase.Number))
                {
                    phase.gameObject.SetActive(true);
                    phase.Init(this);
                }
                else
                    phase.gameObject.SetActive(false);
            }
        }

        #region SAVE-LOAD
        private void Save()
        {
            //PlayerPrefs.SetInt("Phase-1", _unlockedPhaseNumbers.Contains(1) ? 1 : 0);
            PlayerPrefs.SetInt("Phase-2", _unlockedPhaseNumbers.Contains(2) ? 1 : 0);
            PlayerPrefs.SetInt("Phase-3", _unlockedPhaseNumbers.Contains(3) ? 1 : 0);
            PlayerPrefs.SetInt("Phase-4", _unlockedPhaseNumbers.Contains(4) ? 1 : 0);
            PlayerPrefs.SetInt("Phase-5", _unlockedPhaseNumbers.Contains(5) ? 1 : 0);
            PlayerPrefs.SetInt("Phase-6", _unlockedPhaseNumbers.Contains(6) ? 1 : 0);

            //PlayerPrefs.SetInt("UnlockedPhaseNumber", _unlockedPhaseNumber);
            PlayerPrefs.Save();
        }
        private void Load() 
        {
            //if (PlayerPrefs.GetInt("Phase-1") == 1 && !_unlockedPhaseNumbers.Contains(1))
            //    _unlockedPhaseNumbers.Add(1);
            if (PlayerPrefs.GetInt("Phase-2") == 1 && !_unlockedPhaseNumbers.Contains(2))
                _unlockedPhaseNumbers.Add(2);
            if (PlayerPrefs.GetInt("Phase-3") == 1 && !_unlockedPhaseNumbers.Contains(3))
                _unlockedPhaseNumbers.Add(3);
            if (PlayerPrefs.GetInt("Phase-4") == 1 && !_unlockedPhaseNumbers.Contains(4))
                _unlockedPhaseNumbers.Add(4);
            if (PlayerPrefs.GetInt("Phase-5") == 1 && !_unlockedPhaseNumbers.Contains(5))
                _unlockedPhaseNumbers.Add(5);
            if (PlayerPrefs.GetInt("Phase-6") == 1 && !_unlockedPhaseNumbers.Contains(6))
                _unlockedPhaseNumbers.Add(6);

            //_unlockedPhaseNumber = PlayerPrefs.GetInt("UnlockedPhaseNumber", 1);
        }
        #endregion
    }
}
