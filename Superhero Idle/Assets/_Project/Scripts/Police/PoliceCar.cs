using System;
using UnityEngine;
using ZestCore.Ai;
using ZestGames;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.AI;

namespace SuperheroIdle
{
    public class PoliceCar : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private Transform _leftDropTransform, _rightDropTransform;
        public Transform LeftDropTransform => _leftDropTransform;
        public Transform RightDropTransform => _rightDropTransform;
        private List<Police> _policeMen = new List<Police>();
        public List<Police> PoliceMen => _policeMen;

        [Header("-- SETUP --")]
        [SerializeField] private Transform startTransform;
        [SerializeField] private Transform endTransform;
        [SerializeField] private float speed = 10f;
        [SerializeField] private Transform[] spawnPoints;

        [Header("-- POLICEMAN SPAWN SETUP --")]
        private Criminal _activatorCriminal = null;

        private Vector3 _exitPosition;
        private bool _targetReached;
        private MeshRenderer[] _meshes;

        public Action OnStartedMoving, OnStartedIdling, OnStartedLeaving;

        private void Init()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            _agent.speed = speed;

            if (_meshes == null)
                _meshes = GetComponentsInChildren<MeshRenderer>();

            DisableMesh();
            _policeMen.Clear();

            
        }

        private void OnEnable()
        {
            Init();

            _leftDropTransform = transform.GetChild(transform.childCount - 2);
            _rightDropTransform = transform.GetChild(transform.childCount - 1);

            OnStartedLeaving += Leave;
        }

        private void OnDisable()
        {
            _activatorCriminal = null;
            OnStartedLeaving -= Leave;

            transform.DOKill();
        }

        private void Update()
        {
            if (_activatorCriminal && Operation.IsTargetReached(transform, _activatorCriminal.transform.position, 15f) && !_targetReached)
            {
                _agent.SetDestination(transform.position);
                _targetReached = true;
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
                _agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                OnStartedIdling?.Invoke();
                SpawnPoliceMan();
            }

            if (Operation.IsTargetReached(transform, _exitPosition, 5f))
            {
                _agent.SetDestination(transform.position);
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
                transform.position = startTransform.position;
                DisableMesh();
            }
        }

        public void StartTheCar(Criminal criminal)
        {
            _activatorCriminal = criminal;

            PoliceManager.RemoveFreePoliceCar(this);

            EnableMesh();
            Bounce();

            transform.position = startTransform.position;
            transform.rotation = Quaternion.identity;
            OnStartedMoving?.Invoke();

            _agent.isStopped = false;
            _agent.SetDestination(_activatorCriminal.transform.position);
        }

        private void SpawnPoliceMan()
        {
            for (int i = 0; i < 2; i++)
            {
                Police police = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.PoliceMan, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity).GetComponent<Police>();
                if (i == 0)
                    police.SetTargetCriminal(this, _activatorCriminal.LeftCarryPoint, Enums.CarrySide.Left);
                else if (i == 1)
                    police.SetTargetCriminal(this, _activatorCriminal.RightCarryPoint, Enums.CarrySide.Right);

                _policeMen.Add(police);
            }
        }
        private void Leave()
        {
            OnStartedMoving?.Invoke();

            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            _exitPosition = endTransform.position;
            _agent.isStopped = false;
            _agent.SetDestination(_exitPosition);
        }

        private void Bounce()
        {
            transform.DORewind();

            transform.DOShakeRotation(.25f, .5f);
            transform.DOShakeScale(.25f, .5f);
        }

        private void EnableMesh()
        {
            for (int i = 0; i < _meshes.Length; i++)
                _meshes[i].enabled = true;
        }
        private void DisableMesh()
        {
            _targetReached = false;
            _activatorCriminal = null;
            PoliceManager.AddFreePoliceCar(this);

            for (int i = 0; i < _meshes.Length; i++)
                _meshes[i].enabled = false;
        }

        #region PUBLICS
        public void ResetCar(Criminal criminal)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            _activatorCriminal = null;
            DisableMesh();
        }
        #endregion
    }
}
