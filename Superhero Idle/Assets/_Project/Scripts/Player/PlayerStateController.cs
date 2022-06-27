using UnityEngine;
using ZestGames;
using DG.Tweening;

namespace SuperheroIdle
{
    public class PlayerStateController : MonoBehaviour
    {
        private Player _player;
        private Enums.PlayerState _currentState;
        public Enums.PlayerState CurrentState => _currentState;

        private GameObject _heroClothes;
        private SkinnedMeshRenderer _heroMesh;
        private readonly float _heroTime = 30f;
        private float _finishTimeForHero;

        [Header("-- EFFECT SETUP --")]
        [SerializeField] private ParticleSystem changeSmokePS;
        [SerializeField] private GameObject capeObj;

        public void Init(Player player)
        {
            _player = player;
            _heroClothes = transform.GetChild(1).gameObject;
            _heroMesh = _heroClothes.GetComponent<SkinnedMeshRenderer>();
            ChangeToCivillian();

            PlayerEvents.OnChangeToCivillian += ChangeToCivillian;
            PlayerEvents.OnChangeToHero += ChangeToHero;
            PlayerEvents.OnStartFighting += DisableMesh;
            PlayerEvents.OnStopFighting += EnableMesh;
        }

        private void OnDisable()
        {
            PlayerEvents.OnChangeToCivillian -= ChangeToCivillian;
            PlayerEvents.OnChangeToHero -= ChangeToHero;
            PlayerEvents.OnStartFighting -= DisableMesh;
            PlayerEvents.OnStopFighting -= EnableMesh;
        }

        private void Update()
        {
            if (GameManager.GameState == Enums.GameState.GameEnded) return;

            if (_currentState == Enums.PlayerState.Hero && Time.time >= _finishTimeForHero && !_player.CrimeHappeningNearby)
                PlayerEvents.OnChangeToCivillian?.Invoke();
        }

        private void ChangeToCivillian()
        {
            _currentState = Enums.PlayerState.Civillian;
            changeSmokePS.Play();

            _heroClothes.SetActive(true);
            capeObj.SetActive(false);

            Bounce();
        }

        private void ChangeToHero()
        {
            _currentState = Enums.PlayerState.Hero;
            changeSmokePS.Play();

            _heroClothes.SetActive(false);
            capeObj.SetActive(true);

            _finishTimeForHero = Time.time + _heroTime;
            Bounce();
        }
        private void Bounce()
        {
            transform.DORewind();
            transform.DOShakeScale(1f, 1f);
        }

        public void DisableMesh(Criminal ignoreThis) => _heroMesh.enabled = false;
        public void EnableMesh(Criminal ignoreThis) => _heroMesh.enabled = true;
    }
}
