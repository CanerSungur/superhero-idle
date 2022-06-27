using UnityEngine;
using ZestGames;

namespace SuperheroIdle
{
    public class Hammer : MonoBehaviour
    {
        private Criminal _criminal;
        private MeshRenderer _meshRenderer;

        public void Init(Criminal criminal)
        {
            _criminal = criminal;
            if (!_meshRenderer)
                _meshRenderer = GetComponent<MeshRenderer>();

            DisableMesh();

            _criminal.OnProceedAttack += ProceedAttack;
            _criminal.OnRunAway += DisableMesh;
            _criminal.OnDefeated += DisableMesh;
        }

        private void OnDisable()
        {
            _criminal.OnProceedAttack -= ProceedAttack;
            _criminal.OnRunAway -= DisableMesh;
            _criminal.OnDefeated -= DisableMesh;
        }

        private void ProceedAttack(Enums.CriminalAttackType attackType)
        {
            if (attackType == Enums.CriminalAttackType.ATM)
                EnableMesh();
        }
        private void EnableMesh() => _meshRenderer.enabled = true;
        private void DisableMesh() => _meshRenderer.enabled = false;
        private void DisableMesh(bool ignoreThis) => _meshRenderer.enabled = false;
    }
}
