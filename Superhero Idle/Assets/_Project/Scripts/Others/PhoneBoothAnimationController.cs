using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class PhoneBoothAnimationController : MonoBehaviour
    {
        private PhoneBooth _phoneBooth;
        private Animator _animator;

        #region ANIMATION PARAMETERS
        private readonly int _enterID = Animator.StringToHash("Enter");
        private readonly int _exitID = Animator.StringToHash("Exit");
        private readonly int _exitSuccessfullyID = Animator.StringToHash("ExitSuccessful");
        #endregion

        public void Init(PhoneBooth phoneBooth)
        {
            _phoneBooth = phoneBooth;
            _animator = GetComponent<Animator>();

            PlayerEvents.OnEnterPhoneBooth += Entered;
            PlayerEvents.OnExitPhoneBooth += Exit;
            PlayerEvents.OnExitPhoneBoothSuccessfully += ExitSuccessfully;
        }

        private void OnDisable()
        {
            PlayerEvents.OnEnterPhoneBooth -= Entered;
            PlayerEvents.OnExitPhoneBooth -= Exit;
            PlayerEvents.OnExitPhoneBoothSuccessfully -= ExitSuccessfully;
        }

        private void Entered(PhoneBooth phoneBooth)
        {
            if (phoneBooth != _phoneBooth) return;
            _animator.SetTrigger(_enterID);
        }
        private void Exit(PhoneBooth phoneBooth)
        {
            if (phoneBooth != _phoneBooth) return;
            _animator.SetTrigger(_exitID);
        }
        private void ExitSuccessfully(PhoneBooth phoneBooth)
        {
            if (phoneBooth != _phoneBooth) return;
            _animator.SetTrigger(_exitSuccessfullyID);
        }
    }
}
