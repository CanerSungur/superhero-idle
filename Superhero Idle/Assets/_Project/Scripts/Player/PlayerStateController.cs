using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class PlayerStateController : MonoBehaviour
    {
        private Enums.PlayerState _currentState;
        public Enums.PlayerState CurrentState => _currentState;

        private GameObject _civillian, _hero;
        private readonly float _heroTime = 5f;
        private float _finishTimeForHero;

        [Header("-- EFFECT SETUP --")]
        [SerializeField] private ParticleSystem changeSmokePS;

        public void Init(Player player)
        {
            _civillian = transform.GetChild(0).gameObject;
            _hero = transform.GetChild(1).gameObject;
            ChangeToCivillian();

            PlayerEvents.OnChangeToCivillian += ChangeToCivillian;
            PlayerEvents.OnChangeToHero += ChangeToHero;
        }

        private void OnDisable()
        {
            PlayerEvents.OnChangeToCivillian -= ChangeToCivillian;
            PlayerEvents.OnChangeToHero -= ChangeToHero;
        }

        private void Update()
        {
            if (GameManager.GameState == Enums.GameState.GameEnded) return;

            if (_currentState == Enums.PlayerState.Hero && Time.time >= _finishTimeForHero)
                PlayerEvents.OnChangeToCivillian?.Invoke();
        }

        private void ChangeToCivillian()
        {
            _currentState = Enums.PlayerState.Civillian;
            changeSmokePS.Play();

            _civillian.SetActive(true);
            _hero.SetActive(false);
        }

        private void ChangeToHero()
        {
            _currentState = Enums.PlayerState.Hero;
            changeSmokePS.Play();

            _civillian.SetActive(false);
            _hero.SetActive(true);

            _finishTimeForHero = Time.time + _heroTime;
        }
    }
}
