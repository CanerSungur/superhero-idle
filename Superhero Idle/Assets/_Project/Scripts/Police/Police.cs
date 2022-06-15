using UnityEngine;
using ZestCore.Ai;

namespace SuperheroIdle
{
    public class Police : MonoBehaviour
    {
        private Criminal _targetCriminal = null;
        private PoliceCar _spawnedCar = null;
        private bool _tookTheCriminal, _droppedTheCriminal;

        public void SetTargetCriminal(Criminal criminal, PoliceCar policeCar)
        {
            _targetCriminal = criminal;
            _spawnedCar = policeCar;
            _tookTheCriminal = _droppedTheCriminal = false;
        }

        private void Update()
        {
            if (!_targetCriminal || !_spawnedCar) return;

            if (Operation.IsTargetReached(transform, _targetCriminal.transform.position, 1f) && !_tookTheCriminal)
                TakeCriminal();
            else
                WalkToCriminal();

            if (Operation.IsTargetReached(transform, _spawnedCar.transform.position, 5f) && !_droppedTheCriminal)
                DropCriminal();
            else
                WalkToCar();
        }

        private void WalkToCriminal()
        {
            if (_tookTheCriminal) return;
            Navigation.MoveTransform(transform, _targetCriminal.transform.position, 2f);
        }
        private void WalkToCar()
        {
            if (_droppedTheCriminal || !_tookTheCriminal) return;
            Navigation.MoveTransform(transform, _spawnedCar.transform.position, 2f);
        }
        private void TakeCriminal()
        {
            _tookTheCriminal = true;
            Debug.Log("Took Criminal");
        }
        private void DropCriminal()
        {
            _droppedTheCriminal = true;
            Debug.Log("Dropped Criminal");
        }
    }
}
