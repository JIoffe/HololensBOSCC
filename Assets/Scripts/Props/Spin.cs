using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Unity.BOSCC.Props
{
    /// <summary>
    /// Just keep spinning, just keep spinning...
    /// </summary>
    public class Spin : MonoBehaviour
    {
        [System.Serializable]
        public class SpinSpeed
        {
            public float pitch;
            public float yaw;
            public float roll;
        }

        [Tooltip("The speed that this object will rotate on each axis, in degrees per second")]
        public SpinSpeed spinSpeed;

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(spinSpeed.pitch * Time.deltaTime, spinSpeed.yaw * Time.deltaTime, spinSpeed.roll * Time.deltaTime);
        }
    }
}