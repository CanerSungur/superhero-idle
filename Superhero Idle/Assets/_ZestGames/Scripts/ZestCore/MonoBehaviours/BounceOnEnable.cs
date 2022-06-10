using UnityEngine;
using DG.Tweening;

namespace ZestGames
{
    public class BounceOnEnable : MonoBehaviour
    {
        private void OnEnable()
        {
            transform.DORewind();
            transform.DOShakeRotation(.5f, .5f);
            transform.DOShakeScale(.5f, .5f);
        }
    }
}
