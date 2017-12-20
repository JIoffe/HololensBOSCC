using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BM.Unity.BOSCC.GUI
{
    /// <summary>
    /// Extremely naive speech bubble. This does not support stacking of multiple bubbles
    /// </summary>
    public class SpeechBubble : MonoBehaviour
    {
        public float persistence = 5f;

        private Text text;
        private Canvas canvas;
          
        // Use this for initialization
        void Start()
        {
            canvas = GetComponentInChildren<Canvas>();
            text = GetComponentInChildren<Text>();

            canvas.enabled = false;
        }

        public void Echo(string s)
        {
            canvas.enabled = true;
            text.text = s;
            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            yield return new WaitForSeconds(persistence);
            canvas.enabled = false;
        }
    }
}