using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZestGames
{
    public class SwerveInput : MonoBehaviour
    {
        private float _lastFrameFingerPositionX;
        private float _moveFactorX;

        public bool CanTakeInput { get; private set; }
        public float MoveFactorX => _moveFactorX;

        private void Update()
        {
            if (CanTakeInput)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _lastFrameFingerPositionX = Input.mousePosition.x;
                }
                if (Input.GetMouseButton(0))
                {
                    _moveFactorX = Input.mousePosition.x - _lastFrameFingerPositionX;
                    _lastFrameFingerPositionX = Input.mousePosition.x;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    _moveFactorX = 0;
                }
            }
        }
    }
}
