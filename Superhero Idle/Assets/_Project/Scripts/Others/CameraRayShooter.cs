using UnityEngine;

namespace SuperheroIdle
{
    public class CameraRayShooter : MonoBehaviour
    {
        private Camera _camera;
        private Player _player;
        private Building _transparentBuilding = null;
        private Building _hitBuilding = null;

        private RaycastHit _hit;
        private Ray _ray;

        private void Awake()
        {
            _player = FindObjectOfType<Player>();
            _camera = GetComponent<Camera>();
        }

        private void Update()
        {
            _ray = _camera.ScreenPointToRay(_player.transform.position);

            if (Physics.Raycast(_ray, out _hit))
            {
                if (_hit.transform.TryGetComponent(out _hitBuilding))
                {
                    if (_transparentBuilding && _transparentBuilding != _hitBuilding)
                        _transparentBuilding.OnBecomeSolid?.Invoke();

                    _hitBuilding.OnBecomeTransparent?.Invoke();
                    _transparentBuilding = _hitBuilding;
                }
                else
                {
                    if (_transparentBuilding)
                        _transparentBuilding.OnBecomeSolid?.Invoke();

                    _transparentBuilding = _hitBuilding = null;
                }
            }
        }
    }
}
