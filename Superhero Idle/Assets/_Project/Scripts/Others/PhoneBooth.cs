using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class PhoneBooth : MonoBehaviour
    {
        private Animator _animator;
        public Vector3 EntryPosition { get; private set; }
        public bool DoorIsOpen { get; private set; }

        private readonly int _enterID = Animator.StringToHash("Enter");
        private readonly int _exitID = Animator.StringToHash("Exit");
        private readonly int _exitSuccessfullyID = Animator.StringToHash("ExitSuccessful");

        private void Init()
        {
            _animator = GetComponent<Animator>();
            EntryPosition = transform.GetChild(1).position;
            DoorIsOpen = false;

            PlayerEvents.OnEnterPhoneBooth += HeroEntered;
            PlayerEvents.OnExitPhoneBooth += HeroExit;
        }

        private void OnDisable()
        {
            PlayerEvents.OnEnterPhoneBooth -= HeroEntered;
            PlayerEvents.OnExitPhoneBooth -= HeroExit;
        }

        private void OnEnable()
        {
            Init();
        }
        private void HeroEntered()
        {
            _animator.SetTrigger(_enterID);
            DoorIsOpen = true;
        }
        private void HeroExit() => _animator.SetTrigger(_exitID);
        public void HeroExitSuccessfully()
        {
            _animator.SetTrigger(_exitSuccessfullyID);
            DoorIsOpen = false;
        }
    }
}
