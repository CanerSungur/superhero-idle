using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class PoliceAnimationController : MonoBehaviour
    {
        private Police _police;
        private Animator _animator;

        private readonly int _carryID = Animator.StringToHash("Carry");
        private readonly int _dropID = Animator.StringToHash("Drop");
        private readonly int _sideIndexID = Animator.StringToHash("SideIndex");

        private readonly int _leftSideIndex = 1;
        private readonly int _rightSideIndex = 2;
        private readonly int _carryLayer = 1;

        public void Init(Police police)
        {
            _police = police;
            _animator = GetComponent<Animator>();
            _animator.SetLayerWeight(_carryLayer, 1f);

            _police.OnTakeCriminal += StartCarrying;
            _police.OnDropCriminal += Drop;
        }

        private void OnDisable()
        {
            if (!_police) return;
            _police.OnTakeCriminal -= StartCarrying;
            _police.OnDropCriminal -= Drop;
        }

        private void StartCarrying()
        {
            SetSideIndex();
            _animator.SetTrigger(_carryID);
        }
        private void SetSideIndex()
        {
            if (_police.CurrentCarrySide == Enums.PoliceManCarrySide.Left)
                _animator.SetInteger(_sideIndexID, _leftSideIndex);
            else if (_police.CurrentCarrySide == Enums.PoliceManCarrySide.Right)
                _animator.SetInteger(_sideIndexID, _rightSideIndex);
        }
        private void Drop()
        {
            _animator.SetLayerWeight(_carryLayer, 0f);
            _animator.SetTrigger(_dropID);
        }
    }
}
