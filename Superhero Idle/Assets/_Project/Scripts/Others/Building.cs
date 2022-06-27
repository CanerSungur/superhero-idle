using System;
using UnityEngine;

namespace SuperheroIdle
{
    public class Building : MonoBehaviour
    {
        [Header("-- MATERIAL SETUP --")]
        [SerializeField] private Material solidMaterial;
        [SerializeField] private Material transparentMaterial;
        private MeshRenderer _meshRenderer;

        public Action OnBecomeSolid, OnBecomeTransparent;

        private void OnEnable()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            MakeItSolid();

            OnBecomeSolid += MakeItSolid;
            OnBecomeTransparent += MakeItTransparent;
        }

        private void OnDisable()
        {
            OnBecomeSolid -= MakeItSolid;
            OnBecomeTransparent -= MakeItTransparent;
        }

        private void MakeItSolid() => _meshRenderer.material = solidMaterial;
        private void MakeItTransparent() => _meshRenderer.material = transparentMaterial;
    }
}
