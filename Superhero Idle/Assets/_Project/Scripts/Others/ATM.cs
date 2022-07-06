using UnityEngine;
using System;
using ZestCore.Utility;
using DG.Tweening;
using UnityEngine.UI;

namespace SuperheroIdle
{
    public class ATM : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private float reactivationTime = 10f;

        [Header("-- EFFECT SETUP --")]
        [SerializeField] private ParticleSystem hitSmokePS;
        [SerializeField] private ParticleSystem hitMoneyPS;
        [SerializeField] private ParticleSystem brokenSmokePS;
        [SerializeField] private ParticleSystem brokenSparklePS;

        [Header("-- CANVAS SETUP --")]
        [SerializeField] private Image recoverFillImage;
        [SerializeField] private Transform recoverCanvasTransform;

        #region EVENTS
        public Action OnGetAttacked, OnDefeated, OnGetHit, OnRescued;
        #endregion

        private void OnEnable()
        {
            CharacterManager.AddATM(this);
            recoverCanvasTransform.gameObject.SetActive(false);

            OnGetAttacked += GetAttacked;
            OnDefeated += Defeated;
            OnGetHit += GetHit;
            OnRescued += Rescued;
        }

        private void OnDisable()
        {
            CharacterManager.RemoveATM(this);

            OnGetAttacked -= GetAttacked;
            OnDefeated -= Defeated;
            OnGetHit -= GetHit;
            OnRescued -= Rescued;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                Defeated();
            }
        }

        private void GetHit()
        {
            hitSmokePS.Play();
            hitMoneyPS.Play();
        }
        private void GetAttacked()
        {
            //Debug.Log("Atm is getting attacked");
        }
        private void Defeated()
        {
            EnableRecoverCanvas();
            Delayer.DoActionAfterDelay(this, reactivationTime, () =>
            {
                CharacterManager.AddATM(this);
                brokenSmokePS.Stop();
                brokenSparklePS.Stop();
                Bounce();
            });

            brokenSmokePS.Play();
            brokenSparklePS.Play();
        }
        private void Rescued() => CharacterManager.AddATM(this);
        private void Bounce()
        {
            transform.DORewind();

            //transform.DOShakePosition(.25f, .25f);
            //transform.DOShakeRotation(.25f, .5f);
            transform.DOShakeScale(1f, .15f);
        }

        #region RECOVER CANVAS FUNCTIONS
        private void EnableRecoverCanvas()
        {
            recoverCanvasTransform.gameObject.SetActive(true);
            recoverCanvasTransform.DOShakeScale(0.5f, 0.05f);
            DOVirtual.Float(0f, 1f, reactivationTime, r => {
                recoverFillImage.fillAmount = r;
            }).OnComplete(() => DisableRecoverCanvas());
        }
        private void DisableRecoverCanvas()
        {
            recoverCanvasTransform.DOShakeScale(0.5f, 0.05f).OnComplete(() =>{
                recoverCanvasTransform.DOKill();
                recoverCanvasTransform.gameObject.SetActive(false);
            });
        }
        #endregion
    }
}
