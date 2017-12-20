using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity;

namespace BM.Unity.BOSCC.Input
{
    /// <summary>
    /// Manipulation Handler is a single source of response to manipulation
    /// events thrown by the Holotoolkit Input Manager
    /// This allows us to use our own method of selecting objects
    /// </summary>
    public class ManipulationHandler : Singleton<ManipulationHandler>, IManipulationHandler
    {
        public IManipulable ManipulationTarget { get; set; }

        private void Start()
        {
            //Add as a "Global Listener" so that it will always respond to manipulation events
            var inputManager = InputManager.Instance;

            if (inputManager != null)
                inputManager.AddGlobalListener(gameObject);
        }

        public void OnManipulationCanceled(ManipulationEventData eventData)
        {
            if (ManipulationTarget != null)
            {
                ManipulationTarget.OnManipulationCanceled();
                ManipulationTarget = null;
            }
        }

        public void OnManipulationCompleted(ManipulationEventData eventData)
        {
            if (ManipulationTarget != null)
            {
                ManipulationTarget.OnManipulationCompleted();
                ManipulationTarget = null;
            }
        }

        public void OnManipulationStarted(ManipulationEventData eventData)
        {
            if (ManipulationTarget != null)
                ManipulationTarget.OnManipulationStarted();
        }

        public void OnManipulationUpdated(ManipulationEventData eventData)
        {
            if (ManipulationTarget != null)
                ManipulationTarget.Manipulate(eventData.CumulativeDelta);
        }

    }
}