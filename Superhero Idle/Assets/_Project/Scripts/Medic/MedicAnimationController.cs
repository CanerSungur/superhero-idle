using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class MedicAnimationController : MonoBehaviour
    {
        private Medic _medic;
        private Animator _animator;

        private readonly int _carryID = Animator.StringToHash("Carry");
        private readonly int _dropID = Animator.StringToHash("Drop");
        private readonly int _sideIndexID = Animator.StringToHash("SideIndex");

        private readonly int _leftSideIndex = 1;
        private readonly int _rightSideIndex = 2;
        private readonly int _carryLayer = 1;

        public void Init(Medic medic)
        {
            _medic = medic;
            _animator = GetComponent<Animator>();
            _animator.SetLayerWeight(_carryLayer, 1f);

            _medic.OnTakeCivillian += StartCarrying;
            _medic.OnDropCivillian += Drop;
        }

        private void OnDisable()
        {
            if (!_medic) return;
            _medic.OnTakeCivillian-= StartCarrying;
            _medic.OnDropCivillian -= Drop;
        }

        private void StartCarrying()
        {
            SetSideIndex();
            _animator.SetTrigger(_carryID);
        }
        private void SetSideIndex()
        {
            if (_medic.CurrentCarrySide == Enums.CarrySide.Left)
                _animator.SetInteger(_sideIndexID, _leftSideIndex);
            else if (_medic.CurrentCarrySide == Enums.CarrySide.Right)
                _animator.SetInteger(_sideIndexID, _rightSideIndex);
        }
        private void Drop()
        {
            _animator.SetLayerWeight(_carryLayer, 0f);
            _animator.SetTrigger(_dropID);
        }
    }
}
