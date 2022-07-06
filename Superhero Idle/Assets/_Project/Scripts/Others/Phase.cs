using UnityEngine;
using ZestGames;
using ZestCore.Utility;
using System.Collections.Generic;

namespace SuperheroIdle
{
    public class Phase : MonoBehaviour
    {
        private PhaseManager _phaseManager;
        private Collider _collider;
        public Collider Collider => _collider == null ? _collider = transform.GetChild(0).GetComponent<Collider>() : _collider;

        [Header("-- SETUP --")]
        [SerializeField] private int number;
        public int Number => number;

        [Header("-- SPAWN SETUP --")]
        [SerializeField] private int maxCivillianCount = 50;
        [SerializeField] private int maxCriminalCount = 20;

        [Header("-- CRIME SETUP --")]
        [SerializeField] private int maxCrimeCount = 5;
        private int _currentCrimeCount = 0;
        public bool CanCrimeHappen => _currentCrimeCount < maxCrimeCount;

        [Header("-- PHASE UNLOCKER SETUP --")]
        [SerializeField] private PhaseUnlocker[] phaseUnlockers;
        public bool IsUnlocked { get; private set; }

        #region CIVILLIAN LIST SYSTEM
        private List<Civillian> _activeCivillians;
        public List<Civillian> ActiveCivillians => _activeCivillians == null ? _activeCivillians = new List<Civillian>() : _activeCivillians;
        public void AddActiveCivillian(Civillian civillian)
        {
            if (!ActiveCivillians.Contains(civillian))
                ActiveCivillians.Add(civillian);
        }
        public void RemoveActiveCivillian(Civillian civillian)
        {
            if (ActiveCivillians.Contains(civillian))
                ActiveCivillians.Remove(civillian);
        }
        #endregion

        #region CRIMINAL LIST SYSTEM
        private List<Criminal> _activeCriminals;
        public List<Criminal> ActiveCriminals => _activeCriminals == null ? _activeCriminals = new List<Criminal>() : _activeCriminals;
        public void AddActiveCriminal(Criminal criminal)
        {
            if (!ActiveCriminals.Contains(criminal))
                ActiveCriminals.Add(criminal);
        }
        public void RemoveActiveCriminal(Criminal criminal)
        {
            if (ActiveCriminals.Contains(criminal))
                ActiveCriminals.Remove(criminal);
        }
        #endregion

        public void Init(PhaseManager phaseManager)
        {
            _phaseManager = phaseManager;
            IsUnlocked = _phaseManager.UnlockedPhaseNumbers.Contains(number);

            for (int i = 0; i < maxCivillianCount; i++)
                SpawnCivillian();

            for (int i = 0; i < maxCriminalCount; i++)
                SpawnCriminal();

            //Load();
            InitializePhaseUnlockers();

            PeopleEvents.OnCivillianDecreased += SpawnCivillian;
            PeopleEvents.OnCriminalDecreased += SpawnCriminal;

            CrimeEvents.OnCrimeStarted += CrimeStarted;
            CrimeEvents.OnCrimeEnded += CrimeEnded;
        }

        private void OnDisable()
        {
            PeopleEvents.OnCivillianDecreased -= SpawnCivillian;
            PeopleEvents.OnCriminalDecreased -= SpawnCriminal;

            CrimeEvents.OnCrimeStarted -= CrimeStarted;
            CrimeEvents.OnCrimeEnded -= CrimeEnded;
        }

        private void InitializePhaseUnlockers()
        {
            for (int i = 0; i < phaseUnlockers.Length; i++)
            {
                PhaseUnlocker phaseUnlocker = phaseUnlockers[i];

                if (_phaseManager.UnlockedPhaseNumbers.Contains(phaseUnlocker.PhaseToBeUnlocked.Number))
                    phaseUnlocker.gameObject.SetActive(false);
                else
                {
                    phaseUnlocker.gameObject.SetActive(true);
                    phaseUnlocker.Init(this);
                }
            }
        }
        private void CrimeStarted(Phase phase)
        {
            if (phase != this) return;
            _currentCrimeCount++;
        }
        private void CrimeEnded(Phase phase)
        {
            if (phase != this) return;
            _currentCrimeCount--;
        }
        private void SpawnCivillian()
        {
            Civillian civillian = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Civillian, RNG.RandomPointInBounds(Collider.bounds), Quaternion.identity).GetComponent<Civillian>();
            AddActiveCivillian(civillian);
            civillian.SetBelongedPhase(this);
        }
        private void SpawnCriminal()
        {
            Criminal criminal = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Criminal, RNG.RandomPointInBounds(Collider.bounds), Quaternion.identity).GetComponent<Criminal>();
            AddActiveCriminal(criminal);
            criminal.SetBelongedPhase(this);
        }

        //#region SAVE-LOAD FUNCTIONS
        //private void Save()
        //{
        //    PlayerPrefs.SetInt($"Phase-{number}", IsUnlocked == true ? 1 : 0);
        //    PlayerPrefs.Save();
        //}
        //private void Load()
        //{
        //    IsUnlocked = PlayerPrefs.GetInt($"Phase-{number}") == 1;
        //}
        //#endregion
    }
}
