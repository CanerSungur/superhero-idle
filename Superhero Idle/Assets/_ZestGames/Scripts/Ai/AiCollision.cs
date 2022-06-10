using UnityEngine;
using System;

namespace ZestGames
{
    [RequireComponent(typeof(Collider))]
    public class AiCollision : MonoBehaviour
    {
        public event Action OnSomethingHitFront, OnSomethingHitBack;

        public void Init(Ai ai)
        {

        }

        private void OnTriggerEnter(Collider other)
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            Vector3 relativePosition = transform.InverseTransformPoint(collision.GetContact(0).point);
            if (relativePosition.z < 0)
                OnSomethingHitFront?.Invoke();
            else
                OnSomethingHitBack?.Invoke();
        }
    }
}
