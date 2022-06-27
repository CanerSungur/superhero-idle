using System;
using UnityEngine;
using ZestCore.Ai;
using ZestCore.Utility;
using ZestGames;
using DG.Tweening;

namespace SuperheroIdle
{
    public class Medic : MonoBehaviour
    {
        private MedicAnimationController _animationController;

        private Enums.CarrySide _currentCarrySide;
        public Enums.CarrySide CurrentCarrySide => _currentCarrySide;

        private Transform _targetCivillianTransform;
        private Ambulance _spawnedCar = null;
        private bool _tookTheCivillian, _droppedTheCivillian;

        public Action OnTakeCivillian, OnDropCivillian;

        private void Init()
        {
            if (!_animationController)
                _animationController = GetComponent<MedicAnimationController>();

            _animationController.Init(this);
            OnDropCivillian += DropCivillian;

            Bounce();
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            OnDropCivillian -= DropCivillian;
        }

        public void SetTargetCivillian(Ambulance ambulance, Transform targetCivillianTransform, Enums.CarrySide carrySide)
        {
            _currentCarrySide = carrySide;
            _targetCivillianTransform = targetCivillianTransform;
            _spawnedCar = ambulance;
            _tookTheCivillian = _droppedTheCivillian = false;
        }

        private void Update()
        {
            if (!_spawnedCar) return;

            if (Operation.IsTargetReached(transform, _targetCivillianTransform.position, 1f) && !_tookTheCivillian)
                TakeCivillian();
            else
                WalkToCivillian();
        }

        private void WalkToCivillian()
        {
            if (_tookTheCivillian) return;
            Navigation.MoveTransform(transform, _targetCivillianTransform.position, 2f);
            Navigation.LookAtTarget(transform, _targetCivillianTransform.position);
        }
        private void WalkToCar()
        {
            if (_droppedTheCivillian || !_tookTheCivillian) return;
            Navigation.MoveTransform(transform, _spawnedCar.transform.position, 0.5f);
            Navigation.LookAtTarget(transform, _spawnedCar.transform.position);
        }
        private void TakeCivillian()
        {
            _tookTheCivillian = true;
            OnTakeCivillian?.Invoke();

            transform.SetParent(_targetCivillianTransform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            if (!GetComponentInParent<Civillian>().GettingTakenToAmbulance)
                GetComponentInParent<Civillian>().OnGetTakenToAmbulance?.Invoke(_spawnedCar);
        }
        private void DropCivillian()
        {
            transform.SetParent(null);

            Delayer.DoActionAfterDelay(this, 4f, () =>
            {
                _spawnedCar.OnStartedLeaving?.Invoke();
                transform.DOShakeScale(.5f, 1f).OnComplete(() => gameObject.SetActive(false));
            });
        }
        private void Bounce()
        {
            transform.DORewind();
            transform.DOShakeScale(.5f, 1f);
        }
    }
}
