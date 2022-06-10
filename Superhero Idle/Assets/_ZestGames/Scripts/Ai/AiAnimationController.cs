using UnityEngine;

namespace ZestGames
{
    [RequireComponent(typeof(Animator))]
    public class AiAnimationController : MonoBehaviour
    {
        private Ai _ai;

        #region ANIM PARAMETER SETUP

        private readonly int idleID = Animator.StringToHash("Idle");
        private readonly int runID = Animator.StringToHash("Run");
        private readonly int dieID = Animator.StringToHash("Die");

        #endregion

        public void Init(Ai ai)
        {
            _ai = ai;

            Idle();

            _ai.OnIdle += Idle;
            _ai.OnMove += Run;
            _ai.OnDie += Die;
        }

        private void OnDisable()
        {
            _ai.OnIdle -= Idle;
            _ai.OnMove -= Run;
            _ai.OnDie -= Die;
        }

        private void Idle()
        {
            _ai.Animator.SetBool(idleID, true);
            _ai.Animator.SetBool(runID, false);
            _ai.Animator.SetBool(dieID, false);
        }

        private void Run()
        {
            _ai.Animator.SetBool(idleID, false);
            _ai.Animator.SetBool(runID, true);
            _ai.Animator.SetBool(dieID, false);
        }

        private void Die()
        {
            _ai.Animator.SetBool(idleID, false);
            _ai.Animator.SetBool(runID, false);
            _ai.Animator.SetBool(dieID, true);
        }
    }
}
