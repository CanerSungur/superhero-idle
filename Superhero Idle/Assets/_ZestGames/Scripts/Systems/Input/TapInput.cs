using UnityEngine;

namespace ZestGames
{
    public class TapInput : MonoBehaviour
    {
        private void Update()
        {
            if (GameManager.GameState != Enums.GameState.Started) return;

            if (Input.GetMouseButtonDown(0)) InputEvents.OnTapHappened?.Invoke();
        }
    }
}
