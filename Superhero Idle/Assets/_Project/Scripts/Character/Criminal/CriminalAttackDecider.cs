using System;
using System.Collections;
using UnityEngine;
using ZestCore.Utility;
using ZestGames;

namespace SuperheroIdle
{
    public class CriminalAttackDecider : MonoBehaviour
    {
        private Criminal _criminal;
        private IEnumerator _makeDecisionEnum;
        private WaitForSeconds _waitForDecisionDelay;

        private readonly int _actOfCrimeChance = 30;
        private readonly float _crimeDecisionTimer = 10;
        private float _decisionTime;

        #region PROPERTIES
        public Enums.CriminalAttackType AttackType { get; private set; }
        public Civillian TargetCivillian { get; private set; }
        public ATM TargetAtm { get; private set; }
        public bool CanAttack => GameManager.GameState == Enums.GameState.Started && _criminal && _criminal.Player.StateController.CurrentState == Enums.PlayerState.Civillian && !_criminal.IsAttacking && !_criminal.RunningAway && _criminal.BelongedPhase.CanCrimeHappen && Time.time >= _decisionTime;
        #endregion

        public void Init(Criminal criminal)
        {
            _criminal = criminal;
            _decisionTime = Time.time + _crimeDecisionTimer;

            _waitForDecisionDelay = new WaitForSeconds(_crimeDecisionTimer);
            _makeDecisionEnum = MakeDecision();
            StartCoroutine(_makeDecisionEnum);
        }

        private void OnDisable()
        {
            if (_makeDecisionEnum != null)
                StopCoroutine(_makeDecisionEnum);
        }

        //private void Update()
        //{
        //    if (CanAttack && RNG.RollDice(_actOfCrimeChance))
        //        DecideAttackType();
        //}

        private void DecideAttackType()
        {
            int attackCount = Enum.GetNames(typeof(Enums.CriminalAttackType)).Length;
            AttackType = (Enums.CriminalAttackType)UnityEngine.Random.Range(0, attackCount);

            ActivateAttack();
        }
        private void ActivateAttack()
        {
            if (AttackType == Enums.CriminalAttackType.Civillian)
                AttackCivillian();
            else if (AttackType == Enums.CriminalAttackType.ATM)
                AttackATM();

            CharacterManager.AddCriminalCommitingCrime(_criminal);
            CrimeEvents.OnCrimeStarted?.Invoke(_criminal.BelongedPhase);
        }
        private void AttackCivillian()
        {
            TargetCivillian = FindRandomCivillian();
            if (TargetCivillian == null) return;
            _criminal.OnProceedAttack?.Invoke(AttackType);
            _criminal.SetAttackState(true);
        }
        private void AttackATM()
        {
            TargetAtm = FindClosestAtm();
            if (TargetAtm == null) return;
            _criminal.OnProceedAttack?.Invoke(AttackType);
            _criminal.SetAttackState(true);
        }

        private IEnumerator MakeDecision()
        {
            while (true)
            {
                yield return _waitForDecisionDelay;

                if (CanAttack && RNG.RollDice(_actOfCrimeChance))
                    DecideAttackType();
            }
        }

        #region FIND FUNCTIONS
        private Civillian FindRandomCivillian()
        {
            if (_criminal.BelongedPhase.ActiveCivillians == null || _criminal.BelongedPhase.ActiveCivillians.Count == 0) return null;
            Civillian randomCivillian = _criminal.BelongedPhase.ActiveCivillians[UnityEngine.Random.Range(0, _criminal.BelongedPhase.ActiveCivillians.Count)];
            _criminal.BelongedPhase.RemoveActiveCivillian(randomCivillian);
            return randomCivillian;
        }
        private Civillian FindClosestCivillian()
        {
            if (CharacterManager.CivilliansInScene == null || CharacterManager.CivilliansInScene.Count == 0) return null;

            float shortestDistance = Mathf.Infinity;
            Civillian closestCivillian = null;

            for (int i = 0; i < CharacterManager.CivilliansInScene.Count; i++)
            {
                float distanceToTransform = (transform.position - CharacterManager.CivilliansInScene[i].transform.position).sqrMagnitude;
                if (distanceToTransform < shortestDistance && transform != CharacterManager.CivilliansInScene[i])
                {
                    shortestDistance = distanceToTransform;
                    closestCivillian = CharacterManager.CivilliansInScene[i];
                    CharacterManager.RemoveCivillian(CharacterManager.CivilliansInScene[i]);
                }
            }
            return closestCivillian;
        }
        private ATM FindClosestAtm()
        {
            if (CharacterManager.AtmsInScene == null || CharacterManager.AtmsInScene.Count == 0)
            {
                return null;
            }

            float shortestDistance = Mathf.Infinity;
            ATM closestAtm = null;

            for (int i = 0; i < CharacterManager.AtmsInScene.Count; i++)
            {
                float distanceToTransform = (transform.position - CharacterManager.AtmsInScene[i].transform.position).sqrMagnitude;
                if (distanceToTransform < shortestDistance && transform != CharacterManager.AtmsInScene[i])
                {
                    shortestDistance = distanceToTransform;
                    closestAtm = CharacterManager.AtmsInScene[i];
                    CharacterManager.RemoveATM(CharacterManager.AtmsInScene[i]);
                }
            }
            return closestAtm;
        }
        #endregion
    }
}
