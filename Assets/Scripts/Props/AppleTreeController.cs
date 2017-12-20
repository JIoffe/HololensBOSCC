using BM.Unity.BOSCC.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity.InputModule;

namespace BM.Unity.BOSCC.Props
{
    /// <summary>
    /// The apple tree can be shaken by the user and will rotate accordingly
    /// </summary>
    public class AppleTreeController : MonoBehaviour, IManipulable, IInputClickHandler
    {
        private const float RotationLimit = 30f;

        [Tooltip("The prefab to use for fruit to drop from this tree")]
        public GameObject Fruit;

        [Tooltip("How sensitive this tree is to shaking by the hands")]
        public float ShakeSpeedMultiplier = 5.5f;

        [Tooltip("How frequently, in seconds, that fruit should drop while shaking")]
        public float FruitDropInterval = 0.3f;

        [Tooltip("Clip to play when starting to shake")]
        public AudioClip shakeStartClip;

        private Quaternion InitialRotation { get; set; }
        private bool IsShaking { get; set; }
        private bool HasPlayedAudio { get; set; }
        private float FruitDropTimer { get; set; }


        public void Manipulate(Vector3 cumulativeDelta)
        {
            var dX = Mathf.Max(-RotationLimit, Mathf.Min(RotationLimit, -cumulativeDelta.x * ShakeSpeedMultiplier));
            var dZ = Mathf.Max(-RotationLimit, Mathf.Min(RotationLimit, -cumulativeDelta.z * ShakeSpeedMultiplier));

            transform.rotation = Quaternion.Euler(-dZ, 0, dX) * InitialRotation;
        }

        public void OnManipulationCanceled()
        {
            transform.rotation = InitialRotation;
            IsShaking = false;
        }

        public void OnManipulationCompleted()
        {
            transform.rotation = InitialRotation;
            IsShaking = false;
        }

        public void OnManipulationStarted()
        {
            if (!HasPlayedAudio)
            {
                var audioSource = GetComponentInChildren<AudioSource>();
                if (audioSource != null && shakeStartClip != null)
                    audioSource.PlayOneShot(shakeStartClip);
            }
            InitialRotation = transform.rotation; ;
            IsShaking = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (!IsShaking)
            {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, InitialRotation, 0.6f * Time.deltaTime);
                return;
            }

            FruitDropTimer -= Time.deltaTime;

            if (FruitDropTimer <= 0f)
            {
                DropFruit();
                FruitDropTimer = FruitDropInterval;
            }
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            var manipulationHandler = ManipulationHandler.Instance;
            if (manipulationHandler != null)
                manipulationHandler.ManipulationTarget = this;
        }

        private void DropFruit()
        {
            var droppedFruit = Instantiate(Fruit);
            droppedFruit.transform.position = transform.position + new Vector3(0, 1f, 0);

            var rotation = transform.rotation.eulerAngles.normalized;
            var force = rotation * 4.0f;

            var rigidBody = droppedFruit.GetComponent<Rigidbody>();
            rigidBody.AddForce(force);
        }
    }
}