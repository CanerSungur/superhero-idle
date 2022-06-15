using UnityEngine;
using System;
using ZestCore.Ai;
using ZestCore.Utility;
using ZestGames;

namespace SuperheroIdle
{
    public class PoliceCar : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;

        private Criminal _currentCriminal = null;
        private Vector3 _currentTargetPosition;
        private bool _targetReached;

        public bool Idle { get; private set; }

        private void Awake()
        {
            CharacterManager.AddPoliceCar(this);
            Idle = false;
        }

        private void OnDisable()
        {
            Idle = true;
        }

        private void Update()
        {
            if (Idle) return;

            if (Operation.IsTargetReached(transform, _currentTargetPosition, 25f) && !_targetReached)
            {
                _targetReached = true;
                SpawnPoliceMan();

                Delayer.DoActionAfterDelay(this, 20f, () =>
                {
                    _currentTargetPosition = transform.forward;
                    _targetReached = false;
                    Delayer.DoActionAfterDelay(this, 5f, () => gameObject.SetActive(false));
                });
            }
            else
                Motor(_currentTargetPosition);
        }

        private void Motor(Vector3 position)
        {
            if (_targetReached) return;
            Navigation.MoveTransform(transform, position, 5f);
        }
        public void SetCriminalTarget(Criminal criminal)
        {
            _currentCriminal = criminal;
            _currentTargetPosition = criminal.transform.position;
            Idle = _targetReached = false;
        }
        private void SpawnPoliceMan()
        {
            Police police = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.PoliceMan, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity).GetComponent<Police>();
            police.SetTargetCriminal(_currentCriminal, this);       
        }
    }
}
