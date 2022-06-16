using System;
using UnityEngine;
using ZestCore.Ai;
using ZestCore.Utility;
using ZestGames;
using DG.Tweening;

namespace SuperheroIdle
{
    public class Police : MonoBehaviour
    {
        private PoliceAnimationController _animationController;
        
        private Enums.PoliceManCarrySide _currentCarrySide;
        public Enums.PoliceManCarrySide CurrentCarrySide => _currentCarrySide;

        private Transform _targetCriminalTransform;
        private PoliceCar _spawnedCar = null;
        private bool _tookTheCriminal, _droppedTheCriminal;

        public Action OnTakeCriminal, OnDropCriminal;

        private void Init()
        {
            if (!_animationController)
                _animationController = GetComponent<PoliceAnimationController>();
        
            _animationController.Init(this);
            OnDropCriminal += DropCriminal;

            Bounce();
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            OnDropCriminal -= DropCriminal;
        }

        public void SetTargetCriminal(PoliceCar policeCar, Transform targetCriminalTransform, Enums.PoliceManCarrySide carrySide)
        {
            _currentCarrySide = carrySide;
            _targetCriminalTransform = targetCriminalTransform;
            _spawnedCar = policeCar;
            _tookTheCriminal = _droppedTheCriminal = false;
        }

        private void Update()
        {
            if (!_spawnedCar) return;

            if (Operation.IsTargetReached(transform, _targetCriminalTransform.position, 1f) && !_tookTheCriminal)
                TakeCriminal();
            else
                WalkToCriminal();
        }

        private void WalkToCriminal()
        {
            if (_tookTheCriminal) return;
            Navigation.MoveTransform(transform, _targetCriminalTransform.position, 2f);
            Navigation.LookAtTarget(transform, _targetCriminalTransform.position);
        }
        private void WalkToCar()
        {
            if (_droppedTheCriminal || !_tookTheCriminal) return;
            Navigation.MoveTransform(transform, _spawnedCar.transform.position, 0.5f);
            Navigation.LookAtTarget(transform, _spawnedCar.transform.position);
        }
        private void TakeCriminal()
        {
            _tookTheCriminal = true;
            OnTakeCriminal?.Invoke();

            transform.parent = _targetCriminalTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            _targetCriminalTransform.GetComponentInParent<Criminal>().OnGetTakenToPoliceCar?.Invoke(_spawnedCar);
        }
        private void DropCriminal()
        {
            transform.parent = null;

            Delayer.DoActionAfterDelay(this, 4f, () =>
            {
                _spawnedCar.OnStartedLeaving?.Invoke();
                transform.DOShakeScale(.5f, 1f).OnComplete(() => gameObject.SetActive(false));
            });
        }
        private void Bounce()
        {
            transform.DORewind();

            //transform.DOShakePosition(.25f, .25f);
            //transform.DOShakeRotation(.25f, .5f);
            transform.DOShakeScale(.5f, 1f);
        }
    }
}
