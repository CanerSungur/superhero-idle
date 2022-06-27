using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using ZestCore.Utility;
using TMPro;

namespace SuperheroIdle
{
    public class CivillianEffectHandler : MonoBehaviour
    {
        private Civillian _civillian;

        #region HELP BUBBLE
        [Header("-- HELP BUBBLE SETUP --")]
        [SerializeField] private Animator helpBubbleAnimator;
        [SerializeField] private TMP_ColorGradient[] colorGradients;
        private readonly string[] helpDialogues = { 
            "*%$#!", "AAARGH!", "HEEELP!", "OUCH!", "NOOOO!"
        };
        private TextMeshProUGUI _helpBubbleText;
        private RectTransform _helpBubbleImageRect;
        private Vector2 _helpBubbleImageDefaultPos;
        #endregion

        #region HERO REACTION BUBBLE

        #endregion

        private readonly WaitForSeconds _waitForChangeDelay = new WaitForSeconds(10f);
        private IEnumerator _changeTextEnum;

        public void Init(Civillian civillian)
        {
            _civillian = civillian;
            _helpBubbleText = helpBubbleAnimator.GetComponentInChildren<TextMeshProUGUI>();
            _helpBubbleImageRect = helpBubbleAnimator.GetComponent<RectTransform>();
            _helpBubbleImageDefaultPos = _helpBubbleImageRect.anchoredPosition;

            _civillian.OnGetAttacked += StartAskingHelp;
            _civillian.OnDefeated += StopAskingHelp;
            _civillian.OnRescued += StopAskingHelp;
        }

        private void OnDisable()
        {
            _civillian.OnGetAttacked -= StartAskingHelp;
            _civillian.OnDefeated -= StopAskingHelp;
            _civillian.OnRescued -= StopAskingHelp;

            transform.DOKill();
        }

        private void StartAskingHelp(Criminal ignoreThis)
        {
            _changeTextEnum = ChangeImageAfterDelay();
            StartCoroutine(_changeTextEnum);
            helpBubbleAnimator.SetBool("Active", true);
        }
        private void StopAskingHelp()
        {
            StopCoroutine(_changeTextEnum);
            helpBubbleAnimator.SetBool("Active", false);
        }
        private void RandomHelpBubbleText()
        {
            _helpBubbleText.text = helpDialogues[Random.Range(0, helpDialogues.Length)];
            _helpBubbleText.colorGradientPreset = colorGradients[Random.Range(0, colorGradients.Length)];
        }

        private void SetRandomRotation()
        {
            _helpBubbleImageRect.anchoredPosition = _helpBubbleImageDefaultPos;

            float random = 0;
            if (RNG.RollDice(50))
                random = -20f;
            else
                random = 20f;

            _helpBubbleImageRect.localEulerAngles = new Vector3(0f, 0f, random);
            //_helpBubbleImageRect.DOLocalRotateQuaternion(Quaternion.Euler(0f, 0f, random), 1f);
            //_helpBubbleImageRect.transform.DOLocalRotate(new Vector3(0f, 0f, random), 1f);
            //_helpBubbleImageRect.DOAnchorPosX(_helpBubbleImageDefaultPos.x + (random * -10), 1f);
            //_helpBubbleImageRect.transform.DOLocalMoveX(_helpBubbleImageRect.transform.position.x + (random * -0.1f), 1f);
        }
        private IEnumerator ChangeImageAfterDelay()
        {
            while (true)
            {
                helpBubbleAnimator.SetBool("Active", false);
                helpBubbleAnimator.SetBool("Active", true);

                RandomHelpBubbleText();
                SetRandomRotation();
                yield return _waitForChangeDelay;
            }
        }
    }
}
