using UnityEngine;
using DG.Tweening;

namespace ZestCore.Utility
{
    public static class DOTweenUtils
    {
        public static void ShakeTransform(Transform transform, float shakeMagnitude = 0.5f)
        {
            transform.DORewind();
            transform.DOShakeRotation(shakeMagnitude, shakeMagnitude);
            transform.DOShakeScale(shakeMagnitude, shakeMagnitude);
        }
    }
}
