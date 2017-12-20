using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BM.Unity.BOSCC.Input
{
    /// <summary>
    /// Targetable behavior will give the user visual feedback about a hologram,
    /// including that it was highlighted and any options available to the user
    /// </summary>
    public class Targetable : MonoBehaviour, IFocusable
    {
        [Tooltip("The material to use when this object is targeted")]
        public Material TargetedMaterial;

        private Renderer[] ActiveRenderers;
        private Dictionary<Renderer, Material[]> MaterialMap;

        public void OnFocusEnter()
        {
            SetTargetedMaterial();
        }

        public void OnFocusExit()
        {
            SetOriginalMaterial();
        }

        // Use this for initialization
        void Start()
        {
            if (TargetedMaterial == null)
                return;

            MaterialMap = new Dictionary<Renderer, Material[]>();

            ActiveRenderers = GetComponentsInChildren<Renderer>();

            foreach (var renderer in ActiveRenderers)
            {
                var originalMaterial = renderer.material;
                var targetedMaterial = new Material(TargetedMaterial);

                if (originalMaterial.HasProperty("_MainTex"))
                {
                    targetedMaterial.mainTexture = originalMaterial.mainTexture;
                    targetedMaterial.SetFloat("_UseMainTex", 1f);
                }

                if (originalMaterial.HasProperty("_Color"))
                {
                    targetedMaterial.color = originalMaterial.color;
                    targetedMaterial.SetFloat("_UseColor", 1f);
                }

                if (renderer is SpriteRenderer)
                {
                    targetedMaterial.SetFloat("_Cull", 0);
                    targetedMaterial.EnableKeyword("IS_SPRITE");
                }

                var materials = new Material[]
                {
                originalMaterial,
                targetedMaterial
                };

                MaterialMap[renderer] = materials;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SetOriginalMaterial()
        {
            SetMaterial(0);
        }

        private void SetTargetedMaterial()
        {
            SetMaterial(1);
        }

        private void SetMaterial(int i)
        {
            if (TargetedMaterial == null)
                return;

            foreach (var renderer in ActiveRenderers)
            {
                renderer.material = MaterialMap[renderer][i];
            }
        }
    }
}