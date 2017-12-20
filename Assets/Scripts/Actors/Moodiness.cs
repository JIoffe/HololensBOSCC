using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Unity.BOSCC.Actors
{
    public class Moodiness : MonoBehaviour
    {
        [Tooltip("Time in seconds that changes to mood will take effect")]
        public float moodReactionSpeed = 1f;

        private Dictionary<SkinnedMeshRenderer, int[]> MoodTargetMap { get; set; }
        private float[] moodWeights;
        private float[] moodRenderWeights;

        // Use this for initialization
        void Start()
        {
            BindMoodTargets();
        }

        // Update is called once per frame
        void Update()
        {
            for(var i = 0; i < moodWeights.Length; ++i)
            {
                moodRenderWeights[i] = Mathf.Lerp(moodRenderWeights[i], moodWeights[i], moodReactionSpeed * Time.deltaTime);
            }

            RenderCurrentMood();
        }

        public void ChangeMood(Mood mood)
        {
            for(var i = 0; i < moodWeights.Length; ++i)
            {
                if (i == (int)mood)
                    moodWeights[i] = 100f;
                else
                    moodWeights[i] = 0f;
            }
        }

        public void ClearMood()
        {
            for (var i = 0; i < moodWeights.Length; ++i)
            {
                moodWeights[i] = 0f;
            }
        }

        private void RenderCurrentMood()
        {
            foreach(var target in MoodTargetMap)
            {
                var mesh = target.Key;
                for (var i = 0; i < moodWeights.Length; ++i)
                {
                    mesh.SetBlendShapeWeight(i, moodRenderWeights[i]);
                }
            }
        }
        private void BindMoodTargets()
        {
            MoodTargetMap = new Dictionary<SkinnedMeshRenderer, int[]>();
            moodWeights = new float[Enum.GetValues(typeof(Mood)).Length];
            moodRenderWeights = new float[moodWeights.Length];

            var skinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach(var skinnedMesh in skinnedRenderers)
            {
                var mesh = skinnedMesh.sharedMesh;
                var nBlendShapes = mesh.blendShapeCount;

                if (nBlendShapes == 0)
                    continue;

                var blendIndices = new int[Enum.GetValues(typeof(Mood)).Length];
                var hasMoodBlendShapes = false;

                for (var i = 0; i < nBlendShapes; ++i)
                {

                    var blendShapeName = mesh.GetBlendShapeName(i).ToLowerInvariant();

                    foreach(var mood in Enum.GetValues(typeof(Mood)))
                    {
                        if (blendShapeName.Contains(mood.ToString().ToLowerInvariant()))
                        {
                            blendIndices[(int)mood] = i;
                            hasMoodBlendShapes = true;
                        }
                    }
                }


                if (!hasMoodBlendShapes)
                    continue;

                MoodTargetMap[skinnedMesh] = blendIndices;
            }
        }
    }
}