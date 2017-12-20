using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BM.Unity.BOSCC.Input
{
    /// <summary>
    /// Behavior that allows an object to be moved around the screen by manipulating the hand
    /// </summary>
    public class Moveable : MonoBehaviour, IInputClickHandler, IManipulable
    {
        [System.Serializable]
        public enum Activation
        {
            OnClick,
            OnFocus
        }

        [Tooltip("When manipulation for this object will be activated")]
        public Activation activation = Activation.OnClick;

        [Tooltip("The proportion with which to move this object with respect to hand movement")]
        public float speedMultiplier = 0.25f;

        public bool IsUnderManipulation { get; set; }

        private Vector3 InitialPosition { get; set; }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            var manipulationHandler = ManipulationHandler.Instance;
            if (manipulationHandler != null && activation == Activation.OnClick)
                manipulationHandler.ManipulationTarget = this;
        }

        //Note that the delta is CUMULATIVE
        public void Manipulate(Vector3 cumulativeDelta)
        {
            this.transform.position = InitialPosition + cumulativeDelta * speedMultiplier;
        }

        public void OnManipulationStarted()
        {
            IsUnderManipulation = true;
            InitialPosition = transform.position;
        }

        public void OnManipulationCanceled()
        {
            IsUnderManipulation = false;
        }

        public void OnManipulationCompleted()
        {
            IsUnderManipulation = false;
        }
    }
}