using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class CrimeDurationHandler : MonoBehaviour
    {
        private Criminal _criminal;
        private float _timer;

        public void Init(Criminal criminal)
        {
            _criminal = criminal;
            _timer = CrimeManager.MaxCrimeDuration;
        }

        private void Update()
        {
            if (GameManager.GameState == Enums.GameState.Started && _criminal.AttackStarted)
            {
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    _criminal.OnRunAway?.Invoke();
                }
            }
        }
    }
}
