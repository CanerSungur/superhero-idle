using UnityEngine;

namespace ZestGames
{
    public class JoystickInput : MonoBehaviour
    {
        [Header("-- INPUT SETUP --")]
        [SerializeField] private Joystick joystick;

        public Vector3 InputValue { get; private set; }
        public bool CanTakeInput => GameManager.GameState == Enums.GameState.Started;

        private void Update()
        {
            if (CanTakeInput)
                InputValue = new Vector3(joystick.Horizontal, 0f, joystick.Vertical);
            else
                InputValue = Vector3.zero;
        }
    }
}
