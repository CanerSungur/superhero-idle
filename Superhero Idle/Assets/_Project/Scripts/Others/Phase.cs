using UnityEngine;
using ZestGames;
using ZestCore.Utility;
using System.Collections.Generic;

namespace SuperheroIdle
{
    public class Phase : MonoBehaviour
    {
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

        #region PHASE UNLOCKER SECTION
        private List<PhaseUnlocker> _phaseUnlockers;
        public List<PhaseUnlocker> PhaseUnlockers => _phaseUnlockers == null ? _phaseUnlockers = new List<PhaseUnlocker>() : _phaseUnlockers;
        public void AddPhase(PhaseUnlocker phaseUnlocker)
        {
            if (!PhaseUnlockers.Contains(phaseUnlocker))
                PhaseUnlockers.Add(phaseUnlocker);
        }
        public void RemovePhase(PhaseUnlocker phaseUnlocker)
        {
            if (PhaseUnlockers.Contains(phaseUnlocker))
                PhaseUnlockers.Remove(phaseUnlocker);
        }
        #endregion

        private void Init()
        {
            for (int i = 0; i < maxCivillianCount; i++)
                SpawnCivillian();

            for (int i = 0; i < maxCriminalCount; i++)
                SpawnCriminal();

            InitializePhaseUnlockers();

            PeopleEvents.OnCivillianDecreased += SpawnCivillian;
            PeopleEvents.OnCriminalDecreased += SpawnCriminal;

            CrimeEvents.OnCrimeStarted += CrimeStarted;
            CrimeEvents.OnCrimeEnded += CrimeEnded;
        }

        private void Start()
        {
            Delayer.DoActionAfterDelay(this, 2f, Init);
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
            for (int i = 0; i < PhaseUnlockers.Count; i++)
                PhaseUnlockers[i].Init(this);
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
    }
}
