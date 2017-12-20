using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Unity.BOSCC.Actors
{
    public class EyeBlink : MonoBehaviour
    {
        public float blinkTime = 0.2f;
        public float blinkDelay = 2f;

        //Map skinned renderers to their eye blink target indices
        private Dictionary<SkinnedMeshRenderer, int> EyeBlinkTargets { get; set; }
        private bool IsBlinking { get; set; }
        private float BlinkTimeFactor { get; set; }

        // Use this for initialization
        void Start()
        {
            BindEyeBlinkTargets();

            if (EyeBlinkTargets.Count > 0)
            {
                BlinkTimeFactor = (1f / blinkTime) * 100f;
                IsBlinking = true;
                StartCoroutine(Blinking());
            }
        }

        private IEnumerator Blinking()
        {
            while (IsBlinking)
            {
                //Pause
                yield return new WaitForSeconds(blinkDelay);

                var blinkAmount = 0f;

                //Blink down
                while (blinkAmount < 100f)
                {
                    blinkAmount += Time.deltaTime * BlinkTimeFactor;
                    ApplyBlink(blinkAmount);
                    yield return null;
                }
                
                //Blink up
                while (blinkAmount >= 0f)
                {
                    blinkAmount -= Time.deltaTime * BlinkTimeFactor;
                    ApplyBlink(blinkAmount);
                    yield return null;
                }
            }
        }

        private void ApplyBlink(float blinkAmount)
        {
            foreach (var target in EyeBlinkTargets)
            {
                target.Key.SetBlendShapeWeight(target.Value, blinkAmount);
            }
        }

        private void BindEyeBlinkTargets()
        {
            EyeBlinkTargets = new Dictionary<SkinnedMeshRenderer, int>();

            var skinnedMeshComponents = GetComponentsInChildren<SkinnedMeshRenderer>();

            //Go through searching to contain "Blink" since sometimes the models have prefixes
            foreach (var skinnedMesh in skinnedMeshComponents)
            {
                var mesh = skinnedMesh.sharedMesh;
                var nBlendShapes = mesh.blendShapeCount;

                if (nBlendShapes == 0)
                    continue;

                for (var i = 0; i < nBlendShapes; ++i)
                {
                    if (mesh.GetBlendShapeName(i).ToLowerInvariant().Contains("blink"))
                    {
                        EyeBlinkTargets[skinnedMesh] = i;
                        break;
                    }
                }
            }
        }
    }
}