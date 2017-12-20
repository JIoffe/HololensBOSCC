using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace BM.Unity.BOSCC.Props
{
    /// <summary>
    /// Controls playback of a note when hitting a piano key
    /// </summary>
    public class PianoKeyController : MonoBehaviour, IInputClickHandler
    {
        [Tooltip("The clip to play when this key is struck")]
        public AudioClip audioClip;

        private AudioSource audioSource;
        private float pressedTime;

        private const float KEY_ANGLE = 60f;
        private const float KEY_PRESS_DURATION = 0.3f;

        // Use this for initialization
        void Start()
        {
            audioSource = GetComponentInParent<AudioSource>();
        }

        private void Update()
        {
            if (pressedTime <= 0f)
                return;

            pressedTime -= Time.deltaTime;
            pressedTime = Mathf.Max(0f, pressedTime);

            float s = Mathf.SmoothStep(0f, KEY_PRESS_DURATION, pressedTime);

            transform.localRotation = Quaternion.Euler(s * KEY_ANGLE, 0, 0);
        }
        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (audioClip != null && audioSource != null)
                audioSource.PlayOneShot(audioClip);

            //Allow key to angle downwards
            pressedTime = KEY_PRESS_DURATION;
        }
    }
}