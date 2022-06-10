using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace ZestGames
{
    public class HealthBar : MonoBehaviour
    {
        private float _currentHp;
        private float _maxHp;

        [Header("-- SETUP --")]
        [SerializeField, Tooltip("Custom gradient if you're not using images for filling. Leave it white when using images!")] private Gradient gradient;
        [SerializeField, Tooltip("Seconds that will take to update the progress bar.")] private float updateSpeedSeconds = 0.5f;
        private Slider _slider;
        private Image _fill;

        public Action OnChange, OnDamage;

        private void OnEnable()
        {
            _slider = GetComponent<Slider>();
            _fill = transform.GetChild(1).GetComponent<Image>();
            InitializeHealthBar();
            UpdateHealth();

            OnDamage += UpdateHealth;
            OnChange += HealthChanged;
        }

        private void OnDisable()
        {
            OnDamage -= UpdateHealth;
            OnChange -= HealthChanged;
        }

        private void InitializeHealthBar()
        {
            _fill.color = gradient.Evaluate(1f);
            _slider.maxValue = _maxHp;
            _slider.value = _currentHp;
        }

        private void UpdateHealth()
        {
            // Do something when hp changes.
            StartCoroutine(SmoothUpdateHealthBar());
            _fill.color = gradient.Evaluate(_slider.normalizedValue);
        }

        private void HealthChanged()
        {
            InitializeHealthBar();
            UpdateHealth();
        }

        private IEnumerator SmoothUpdateHealthBar()
        {
            float preChange = _slider.value;
            float elapsed = 0f;
            while (elapsed < updateSpeedSeconds)
            {
                elapsed += Time.deltaTime;
                Mathf.Lerp(preChange, _currentHp, 0);

                _slider.value = Mathf.Lerp(preChange, _currentHp, elapsed / updateSpeedSeconds);
                yield return null;
            }

            _slider.value = _currentHp;
        }
    }
}
