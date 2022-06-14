using UnityEngine;
using System;

namespace SuperheroIdle
{
    public class Criminal : CharacterBase
    {
        private Civillian _targetCivillian;
        public Civillian TargetCivillian => _targetCivillian;

        #region EVENTS
        public Action OnAttack, OnRunAway;
        public Action OnDecideToAttack;
        #endregion

        private bool _isAttacking = false;
        public bool IsAttacking => _isAttacking;

        private void OnEnable()
        {
            CharacterManager.AddCriminal(this);
            Init();
        }

        private void OnDisable()
        {
            CharacterManager.RemoveCriminal(this);         
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A) && !_isAttacking && CharacterManager.CivilliansInScene.Count > 0)
            {
                _targetCivillian = FindClosestCivillian();
                _isAttacking = true;
                OnDecideToAttack?.Invoke();
            }
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
                }
            }
            return closestCivillian;
        }
    }
}
