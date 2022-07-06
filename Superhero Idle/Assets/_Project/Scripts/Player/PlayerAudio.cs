using System.Collections;
using UnityEngine;
using ZestGames;
using ZestCore.Utility;

namespace SuperheroIdle
{
    public class PlayerAudio : MonoBehaviour
    {
        private Player _player;

        #region COLLECT-SPEND MONEY
        private readonly float _collectTargetPitch = 10f;
        private readonly float _spendTargetPitch = 1f;
        private readonly float _pitchIncrement = 0.1f;
        private float _currentCollectPitch, _currentSpendPitch;

        private bool _collectingMoney, _spendingMoney;
        private readonly float _cooldown = 2f;
        private float _collectTimer, _spendTimer;
        #endregion

        #region PUNCHING
        private readonly WaitForSeconds _punchDelayLong = new WaitForSeconds(0.5f);
        private readonly WaitForSeconds _punchDelayShort = new WaitForSeconds(0.2f);
        private IEnumerator _punchEnum;
        #endregion

        private void Start()
        {
            _player = GetComponent<Player>();
            AudioEvents.OnPlayCollectMoney += HandleCollectMoney;
            AudioEvents.OnPlaySpendMoney += HandleSpendMoney;
            AudioEvents.OnStartPunch += StartPunching;
            AudioEvents.OnStopPunch += StopPunching;

            _currentCollectPitch = 1f;
            _currentSpendPitch = 3f;
            _collectingMoney = _spendingMoney = false;
            _collectTimer = _cooldown;
            _spendTimer = _cooldown;
        }

        private void OnDisable()
        {
            AudioEvents.OnPlayCollectMoney -= HandleCollectMoney;
            AudioEvents.OnPlaySpendMoney -= HandleSpendMoney;
            AudioEvents.OnStartPunch -= StartPunching;
            AudioEvents.OnStopPunch -= StopPunching;
        }

        private void Update()
        {
            if (!_player) return;

            HandleCollectMoneyCooldown();

            HandleSpendMoneyCooldown();
        }

        private void HandleCollectMoney()
        {
            AudioHandler.PlayAudio(Enums.AudioType.Button_Click, 0.8f, _currentCollectPitch);
            _collectTimer = _cooldown;
            _collectingMoney = true;
        }

        private void HandleSpendMoney()
        {
            AudioHandler.PlayAudio(Enums.AudioType.Button_Click, 0.3f, _currentSpendPitch);
            _spendTimer = _cooldown;
            _spendingMoney = true;
        }

        private void HandleCollectMoneyCooldown()
        {
            if (_collectingMoney)
            {
                _collectTimer -= Time.deltaTime;
                if (_collectTimer < 0f)
                {
                    _collectTimer = _cooldown;
                    _collectingMoney = false;
                }

                _currentCollectPitch = Mathf.Lerp(_currentCollectPitch, _collectTargetPitch, _pitchIncrement * Time.deltaTime);
            }
            else
                _currentCollectPitch = 1f;
        }

        private void HandleSpendMoneyCooldown()
        {
            if (_spendingMoney)
            {
                _spendTimer -= Time.deltaTime;
                if (_spendTimer < 0f)
                {
                    _spendTimer = _cooldown;
                    _spendingMoney = false;
                }

                _currentSpendPitch = Mathf.Lerp(_currentSpendPitch, _spendTargetPitch, _pitchIncrement * Time.deltaTime);
            }
            else
                _currentSpendPitch = 3f;
        }

        private void StartPunching()
        {
            _punchEnum = Punch();
            StartCoroutine(_punchEnum);
        }
        private void StopPunching()
        {
            StopCoroutine(_punchEnum);
            StopAllCoroutines();
        }
        private IEnumerator Punch()
        {
            while (true)
            {
                AudioHandler.PlayAudio(Enums.AudioType.Punch, 0.7f);

                if (RNG.RollDice(20))
                    yield return _punchDelayShort;
                else
                    yield return _punchDelayLong;
            }
        }
    }
}
