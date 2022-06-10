using UnityEngine;

namespace ZestGames
{
    public class ProgressBarKnob : MonoBehaviour
    {
        private int _currentPhase = 1;

        [Header("-- KNOB REFERENCES --")]
        [SerializeField] private Animation phase_01_Finished_Anim;
        [SerializeField] private Animation phase_02_Finished_Anim;
        [SerializeField] private Animation phase_03_Finished_Anim;

        [Header("-- LINE REFERENCES --")]
        [SerializeField] private Animation line_01_Anim;
        [SerializeField] private Animation line_02_Anim;

        private void OnEnable()
        {
            GameEvents.OnChangePhase += EnableRelevantKnob;
        }

        private void OnDisable()
        {
            GameEvents.OnChangePhase -= EnableRelevantKnob;
        }

        private void EnableRelevantKnob()
        {
            switch (_currentPhase)
            {
                case 2:
                    phase_01_Finished_Anim.Play();
                    break;
                case 3:
                    phase_02_Finished_Anim.Play();
                    line_01_Anim.Play();
                    break;
                case 4:
                    phase_03_Finished_Anim.Play();
                    line_02_Anim.Play();
                    break;
            }
        }
    }
}
