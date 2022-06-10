using UnityEngine;

namespace ZestGames
{
    public class HoldInput : MonoBehaviour
    {
        private void Update()
        {
            if (GameManager.GameState != Enums.GameState.Started)
            {
                InputEvents.OnTouchStopped?.Invoke();
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                InputEvents.OnTouchStarted?.Invoke();
            }
            if (Input.GetMouseButtonUp(0))
            {
                InputEvents.OnTouchStopped?.Invoke();
            }
        }
    }
}
