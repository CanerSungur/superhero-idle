using UnityEngine;
using Cinemachine;
using System;
using SuperheroIdle;

namespace ZestGames
{
    public class CameraManager : MonoBehaviour
    {
        [Header("-- CAMERA SETUP --")]
        [SerializeField] private CinemachineVirtualCamera gameStartCM;
        [SerializeField] private CinemachineVirtualCamera gameplayCM;
        [SerializeField] private CinemachineVirtualCamera upgradeCM;
        private CinemachineTransposer _gameplayCMTransposer;

        [Header("-- SHAKE SETUP --")]
        private CinemachineBasicMultiChannelPerlin _gameplayCMBasicPerlin;
        private bool _shakeForAWhileStarted = false;
        private float _shakeDuration = 1f;
        private float _shakeTimer;

        public static Action OnShakeCamForAWhile;

        private void Awake()
        {
            _gameplayCMTransposer = gameplayCM.GetCinemachineComponent<CinemachineTransposer>();
            _gameplayCMBasicPerlin = gameplayCM.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _gameplayCMBasicPerlin.m_AmplitudeGain = 0f;
            _shakeTimer = _shakeDuration;

            gameStartCM.Priority = 2;
            gameplayCM.Priority = 1;
            upgradeCM.Priority = 0;
        }

        private void Start()
        {
            GameEvents.OnGameStart += () => gameplayCM.Priority = 3;
            OnShakeCamForAWhile += () => _shakeForAWhileStarted = true;

            UpgradeEvents.OnOpenUpgradeCanvas += EnableUpgradeCanvasCam;
            UpgradeEvents.OnCloseUpgradeCanvas += DisableUpgradeCanvasCam;
            PlayerEvents.OnStartFighting += StartShakingCam;
            PlayerEvents.OnStopFighting += StopShakingCam;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= () => gameplayCM.Priority = 3;
            OnShakeCamForAWhile -= () => _shakeForAWhileStarted = true;
 
            UpgradeEvents.OnOpenUpgradeCanvas -= EnableUpgradeCanvasCam;
            UpgradeEvents.OnCloseUpgradeCanvas -= DisableUpgradeCanvasCam;
            PlayerEvents.OnStartFighting -= StartShakingCam;
            PlayerEvents.OnStopFighting -= StopShakingCam;
        }

        private void Update()
        {
            ShakeCamForAWhile();
        }

        private void EnableUpgradeCanvasCam()
        {
            upgradeCM.Priority = 5;
        }
        private void DisableUpgradeCanvasCam()
        {
            upgradeCM.Priority = 0;
        }
        private void ShakeCamForAWhile()
        {
            if (_shakeForAWhileStarted)
            {
                StartShakingCam();

                _shakeTimer -= Time.deltaTime;
                if (_shakeTimer <= 0f)
                {
                    _shakeForAWhileStarted = false;
                    _shakeTimer = _shakeDuration;

                    StopShakingCam();
                }
            }
        }
        private void StartShakingCam() => _gameplayCMBasicPerlin.m_AmplitudeGain = 1f;
        private void StartShakingCam(Criminal criminal) => _gameplayCMBasicPerlin.m_AmplitudeGain = 0.5f;
        private void StopShakingCam() => _gameplayCMBasicPerlin.m_AmplitudeGain = 0f;
        private void StopShakingCam(Criminal criminal) => _gameplayCMBasicPerlin.m_AmplitudeGain = 0f;
    }
}
