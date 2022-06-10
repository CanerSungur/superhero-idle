using UnityEngine;

namespace ZestGames
{
    public class Billboard : MonoBehaviour
    {
        private Transform _cam;

        private void OnEnable()
        {
            _cam = Camera.main.transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + _cam.forward);
        }
    }
}
