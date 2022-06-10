using UnityEngine;
using UnityEngine.UI;

namespace ZestGames
{
    public class ScrollingBackground : MonoBehaviour
    {
        [Header("-- SETUP --")]
        [SerializeField] private RawImage image;
        [SerializeField] private float x, y;

        private void Update()
        {
            image.uvRect = new Rect(image.uvRect.position + new Vector2(x, y) * Time.deltaTime, image.uvRect.size);
        }
    }
}
