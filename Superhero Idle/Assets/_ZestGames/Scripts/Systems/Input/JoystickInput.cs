using SuperheroIdle;
using UnityEngine;

namespace ZestGames
{
    public class JoystickInput : MonoBehaviour
    {
        private Player _player;
        private Camera _mainCamera;

        [Header("-- INPUT SETUP --")]
        [SerializeField] private Joystick joystick;

        public Vector3 InputValue { get; private set; }
        public bool CanTakeInput => GameManager.GameState == Enums.GameState.Started && Time.time >= _delayedTime;
        private float _delayedTime;
        private readonly float _delayRate = 2f;

        public void Init(Player player)
        {
            _player = player;
            _mainCamera = Camera.main;
            GameEvents.OnGameStart += () => _delayedTime = Time.time + _delayRate;
        }

        private void OnDisable()
        {
            GameEvents.OnGameStart -= () => _delayedTime = Time.time + _delayRate;
        }

        private void Update()
        {
            #region NORMAL INPUT
            if (CanTakeInput)
                InputValue = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
            else
                InputValue = Vector3.zero;
            #endregion

            #region PLAYER DIRECTION INPUT
            //if (CanTakeInput)
            //    InputValue = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0) * new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
            //else
            //    InputValue = Vector3.zero;
            #endregion
        }
    }
}
