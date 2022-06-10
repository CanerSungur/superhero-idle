using UnityEngine;
using DG.Tweening;
using System;

namespace ZestGames
{
    [RequireComponent(typeof(CollectableBase))]
    public class CollectableMovement : MonoBehaviour
    {
        private CollectableBase _collectableBase;

        [Header("-- MOVEMENT SETUP --")]
        [SerializeField] private float speed = 5f;

        // Cam related controls
        private Camera _cam;
        private float _distanceBetweenCamAndPlayer = -5f;
        private Vector3 _collectableHUDWorldPos;

        // Flags
        private bool _triggerMoving = false;
        private bool _coinHasReached = false;
        private bool _startedRotating = false;

        // Events
        public Action OnStartMovement;

        public void Init(CollectableBase collectableBase)
        {
            _collectableBase = collectableBase;
            _cam = Camera.main;
            _triggerMoving = false;

            OnStartMovement += StartMoving;
        }

        private void OnDisable()
        {
            OnStartMovement -= StartMoving;
        }

        private void Update()
        {
            if (_triggerMoving)
            {
                //_distanceBetweenCamAndPlayer = (CharacterPositionHolder.PlayerInScene.transform.position - _cam.transform.position).magnitude;
                _collectableHUDWorldPos = _cam.ScreenToWorldPoint(Hud.CollectableHUDTransform.position + new Vector3(-0.5f, 0f, _distanceBetweenCamAndPlayer));
                Vector3 dir = _collectableHUDWorldPos - transform.position;
                transform.Translate(dir * speed * Time.deltaTime, Space.World);
                //transform.DOMove(coinHUDWorldPos, Coin.MovementTime).SetEase(Ease.InSine);
                if (!_startedRotating)
                {
                    transform.DORotate(new Vector3(0, 360, 0), speed * 0.25f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
                    _startedRotating = true;
                }
                //triggerMoving = false;
            }

            if (Vector3.Distance(transform.position, _collectableHUDWorldPos) <= 1f && !_coinHasReached)
            {
                _triggerMoving = false;
                CollectableEvents.OnCollect?.Invoke(_collectableBase.Value);
                transform.DOScale(0, 0.5f).SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    transform.DOKill();
                    Destroy(gameObject, 1f);
                });
                _coinHasReached = true;
            }
        }

        public void TriggerRewardMovement()
        {
            transform.localScale = Vector3.zero;

            transform.DOScale(1, 1f).SetEase(Ease.OutElastic);
            transform.DOMove(new Vector3(UnityEngine.Random.Range(transform.position.x - .75f, transform.position.x + .75f), UnityEngine.Random.Range(transform.position.y, transform.position.y + 1f), UnityEngine.Random.Range(transform.position.z - .5f, transform.position.z + .5f)), 0.5f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                transform.DOMove(transform.position, 0.5f).OnComplete(() => _triggerMoving = true);
            });
        }

        private void StartMoving() => _triggerMoving = true;
    }
}
