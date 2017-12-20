using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Unity.BOSCC.Input
{
    /// <summary>
    /// Interface for objects that can be manipulated
    /// </summary>
    public interface IManipulable
    {
        //Delta retrieved from device is cumulative
        void Manipulate(Vector3 cumulativeDelta);
        void OnManipulationStarted();
        void OnManipulationCanceled();
        void OnManipulationCompleted();
    }
}