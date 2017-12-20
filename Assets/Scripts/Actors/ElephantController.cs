using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BM.Unity.BOSCC.Actors
{
    /// <summary>
    /// Specifically used for the elephant in the BOSCC 27 Demo
    /// </summary>
    public class ElephantController : MonoBehaviour, IFocusable
    {
        [Tooltip("The clip to play when the elephant is rearing up on its hind legs")]
        public AudioClip roarAudioClip;
        [Tooltip("The clip to play when the elephant is startled")]
        public AudioClip scaredAudioClip;

        private Animator animator;
        private AudioSource audioSource;
        private bool HasRoared { get; set; }
        private bool IsLevitating { get; set; }

        private float MaxLevitationHeight { get; set; }

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            audioSource = GetComponentInChildren<AudioSource>();
        }

        public void OnFocusEnter()
        {
            if (!HasRoared && !IsLevitating)
            {
                audioSource.PlayOneShot(roarAudioClip);
                animator.SetTrigger("Roar");
                HasRoared = true;
            }
        }

        public void OnFocusExit()
        {
            animator.ResetTrigger("Roar");
        }

        public void OnLevitation()
        {
            if (IsLevitating)
                return;

            audioSource.PlayOneShot(scaredAudioClip);

            MaxLevitationHeight = transform.position.y + 1.5f;

            IsLevitating = true;
            StartCoroutine(Levitate());
            animator.SetBool("IsFloating", true);
        }

        IEnumerator Levitate()
        {
            var pos = transform.position;

            while(pos.y < MaxLevitationHeight)
            {
                pos.y += 0.3f * Time.deltaTime;
                transform.position = pos;
                yield return null;
            }

            pos.y = MaxLevitationHeight;
            transform.position = pos;
        }
    }
}