using System;
using UnityEngine;
using ZestCore.Ai;
using ZestGames;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.AI;

namespace SuperheroIdle
{
    public class Ambulance : MonoBehaviour
    {
        #region ANIMATION
        private Animator _animator;
        private readonly int _openID = Animator.StringToHash("Open");
        private readonly int _closeID = Animator.StringToHash("Close");
        #endregion

        private NavMeshAgent _agent;

        #region MEDICS & DROP POINT
        private Transform _dropTransform;
        public Transform DropTransform => _dropTransform;
        private List<Medic> _medics = new List<Medic>();
        public List<Medic> Medics => _medics;
        #endregion

        [Header("-- SETUP --")]
        [SerializeField] private Transform startTransform;
        [SerializeField] private Transform endTransform;
        [SerializeField] private float speed = 10f;
        [SerializeField] private Transform[] spawnPoints;

        [Header("-- MEDIC SPAWN SETUP --")]
        private Civillian _activatorCivillian = null;

        private Vector3 _exitPosition;
        private bool _targetReached;
        private MeshRenderer[] _meshes;

        #region EVENTS
        public Action OnStartedMoving, OnStartedIdling, OnStartedLeaving;
        public Action OnOpenDoor, OnCloseDoor;
        #endregion

        private void Init()
        {
            if (_meshes == null)
            {
                _meshes = GetComponentsInChildren<MeshRenderer>();
                _agent = GetComponent<NavMeshAgent>();
                _animator = GetComponent<Animator>();
            }

            _agent.obstacleAvoidanceType = ObstacleAvoidanceType.MedQualityObstacleAvoidance;
            _agent.speed = speed;

            DisableMesh();
            _medics.Clear();
        }

        private void OnEnable()
        {
            Init();

            _dropTransform = transform.GetChild(transform.childCount - 1);

            OnStartedLeaving += Leave;
            OnOpenDoor += OpenDoor;
            OnCloseDoor += CloseDoor;
        }

        private void OnDisable()
        {
            OnStartedLeaving -= Leave;
            OnOpenDoor -= OpenDoor;
            OnCloseDoor -= CloseDoor;

            transform.DOKill();
        }

        private void Update()
        {
            if (_activatorCivillian && Operation.IsTargetReached(transform, _activatorCivillian.transform.position, 15f) && !_targetReached)
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

        public void StartTheCar(Civillian civillian)
        {
            _activatorCivillian = civillian;

            MedicManager.RemoveFreeAmbulance(this);

            EnableMesh();
            Bounce();

            transform.position = startTransform.position;
            transform.rotation = Quaternion.identity;
            OnStartedMoving?.Invoke();

            _agent.isStopped = false;
            _agent.SetDestination(_activatorCivillian.transform.position);
        }

        private void SpawnPoliceMan()
        {
            for (int i = 0; i < 2; i++)
            {
                Medic medic = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.Medic, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity).GetComponent<Medic>();
                if (i == 0)
                    medic.SetTargetCivillian(this, _activatorCivillian.LeftCarryPoint, Enums.CarrySide.Left);
                else if (i == 1)
                    medic.SetTargetCivillian(this, _activatorCivillian.RightCarrypoint, Enums.CarrySide.Right);

                _medics.Add(medic);
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
            _activatorCivillian = null;
            MedicManager.AddFreeAmbulance(this);

            for (int i = 0; i < _meshes.Length; i++)
                _meshes[i].enabled = false;
        }

        #region ANIMATION FUNCTIONS
        private void OpenDoor() => _animator.SetTrigger(_openID);
        private void CloseDoor() => _animator.SetTrigger(_closeID);
        #endregion

        #region PUBLICS
        public void ResetCar(Civillian civillian)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
            _activatorCivillian = null;
            DisableMesh();
        }
        #endregion
    }
}
