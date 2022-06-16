using System;
using UnityEngine;
using ZestCore.Ai;
using ZestCore.Utility;
using ZestGames;
using DG.Tweening;
using System.Collections.Generic;

namespace SuperheroIdle
{
    public class PoliceCar : MonoBehaviour
    {
        private Transform _leftDropTransform, _rightDropTransform;
        public Transform LeftDropTransform => _leftDropTransform;
        public Transform RightDropTransform => _rightDropTransform;
        private List<Police> _policeMen = new List<Police>();
        public List<Police> PoliceMen => _policeMen;

        [Header("-- SETUP --")]
        [SerializeField] private Transform startTransform;
        [SerializeField] private float speed = 10f;
        [SerializeField] private Transform[] spawnPoints;

        [Header("-- POLICEMAN SPAWN SETUP --")]
        private Criminal _activatorCriminal;

        private Vector3 _targetPosition, _turnPosition;
        private bool _startMoving, _targetReached, _turnReached;
        private float _idlingTime = 2f;

        private MeshRenderer[] _meshes;

        public Action OnStartedMoving, OnStartedIdling, OnStartedLeaving;

        private void Init()
        {
            if (_meshes == null)
                _meshes = GetComponentsInChildren<MeshRenderer>();

            DisableMesh();
            _policeMen.Clear();

            PoliceManager.AddFreePoliceCar(this);
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
            OnStartedLeaving -= Leave;
        }

        private void Update()
        {
            if (_startMoving)
            {
                if (_targetReached)
                {
                    transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
                    Delayer.DoActionAfterDelay(this, 5f, Init);
                }
                else
                {
                    //if (_turnReached)
                    //{
                    //Navigation.MoveTransform(transform, _targetPosition, speed);
                    transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
                    Navigation.LookAtTarget(transform, _targetPosition, 7f);
                    if (Operation.IsTargetReached(transform, _targetPosition, 25f) && !_targetReached)
                    {
                        _targetReached = true;
                        OnStartedIdling?.Invoke();
                        _startMoving = false;
                        //Delayer.DoActionAfterDelay(this, _idlingTime, () => _startMoving = true);
                        //Delayer.DoActionAfterDelay(this, _idlingTime, () => OnStartedMoving?.Invoke());

                        SpawnPoliceMan();
                    }
                    //}
                    //else
                    //{
                    //Navigation.MoveTransform(transform, _turnPosition, speed);
                    //Navigation.LookAtTarget(transform, _turnPosition, 7f);
                    //if (Operation.IsTargetReached(transform, _turnPosition) && !_turnReached)
                    //    _turnReached = true;
                    //}
                }
            }
        }

        public void StartTheCar(Criminal criminal)
        {
            _activatorCriminal = criminal;

            PoliceManager.RemoveFreePoliceCar(this);
            //_turnPosition = _activatorCriminal.TurnPosition;
            _targetPosition = _activatorCriminal.transform.position;

            EnableMesh();
            Bounce();

            transform.position = startTransform.position;
            transform.rotation = Quaternion.identity;
            _startMoving = true;
            OnStartedMoving?.Invoke();
        }

        private void SpawnPoliceMan()
        {
            for (int i = 0; i < 2; i++)
            {
                Police police = ObjectPooler.Instance.SpawnFromPool(Enums.PoolStamp.PoliceMan, spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)].position, Quaternion.identity).GetComponent<Police>();
                if (i == 0)
                    police.SetTargetCriminal(this, _activatorCriminal.LeftCarryPoint, Enums.PoliceManCarrySide.Left);
                else if (i == 1)
                    police.SetTargetCriminal(this, _activatorCriminal.RightCarrypoint, Enums.PoliceManCarrySide.Right);

                _policeMen.Add(police);
            }
        }
        private void Leave()
        {
            _startMoving = true;
            OnStartedMoving?.Invoke();
        }

        private void Bounce()
        {
            transform.DORewind();

            //transform.DOShakePosition(.25f, .25f);
            transform.DOShakeRotation(.25f, .5f);
            transform.DOShakeScale(.25f, .5f);
        }

        private void EnableMesh()
        {
            for (int i = 0; i < _meshes.Length; i++)
                _meshes[i].enabled = true;

            //leftTireMudPS.Play();
            //rightTireMudPS.Play();
        }
        private void DisableMesh()
        {
            _startMoving = _targetReached = false;
            transform.position = startTransform.position;

            for (int i = 0; i < _meshes.Length; i++)
                _meshes[i].enabled = false;

            //leftTireMudPS.Stop();
            //rightTireMudPS.Stop();
        }
    }
}
