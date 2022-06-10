using UnityEngine;
using UnityEngine.EventSystems;

namespace ZestGames
{
    public class TouchToStart : MonoBehaviour, IPointerDownHandler
    {
        public void OnPointerDown(PointerEventData eventData) => GameEvents.OnGameStart?.Invoke();
    }
}
