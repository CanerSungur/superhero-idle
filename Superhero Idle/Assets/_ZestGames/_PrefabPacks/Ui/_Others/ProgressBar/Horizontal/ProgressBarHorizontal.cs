using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZestGames
{

    /// <summary>
    /// Use either UpdateProgressBar in Update or SmoothUpdateProgressBar when something happens.
    /// </summary>
    public class ProgressBarHorizontal : MonoBehaviour
    {
        [Header("-- REFERENCES --")]
        [SerializeField, Tooltip("Considered as start point or focused object through progression i.e. Player.")] private Transform focusTransform;
        [SerializeField, Tooltip("Considered as end point i.e. Finish Line.")] private Transform finishTransform;
        [SerializeField, Tooltip("Custom gradient if you're not using images for filling. Leave it white when using images!")] private Gradient gradient;
        [SerializeField, Tooltip("Seconds that will take to update the progress bar.")] private float updateSpeedSeconds = 0.5f;

        [Header("-- SETUP --")]
        private Slider _slider;
        private Image _fill;
        private float _currentZPosition = 0;
        private float _finishZPosition;

        private void Awake()
        {
            // That's because game is usually progressing on z axis.
            _finishZPosition = finishTransform.position.z;
            _currentZPosition = focusTransform.position.z;

            _slider = GetComponent<Slider>();
            _fill = transform.GetChild(1).GetChild(0).GetComponent<Image>();

            _fill.color = gradient.Evaluate(1f);
            _slider.maxValue = _finishZPosition;
            _slider.minValue = _currentZPosition;

            _slider.value = _currentZPosition;
        }

        private void Update()
        {
            UpdateProgressBar();
        }

        private void CheckIfFocusPassedFinish()
        {
            if (focusTransform.position.z > _currentZPosition)
                _currentZPosition = focusTransform.position.z;
        }

        private void UpdateProgressBar()
        {
            CheckIfFocusPassedFinish();

            _slider.value = _currentZPosition;
            _fill.color = gradient.Evaluate(_slider.normalizedValue);
        }

        private IEnumerator SmoothUpdateProgressBar()
        {
            CheckIfFocusPassedFinish();

            float preChange = _slider.value;
            float elapsed = 0f;
            while (elapsed < updateSpeedSeconds)
            {
                elapsed += Time.deltaTime;
                Mathf.Lerp(preChange, _currentZPosition, 0);

                _slider.value = Mathf.Lerp(preChange, _currentZPosition, elapsed / updateSpeedSeconds);
                yield return null;
            }

            _slider.value = _currentZPosition;
        }
    }
}
