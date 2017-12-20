using BM.Unity.BOSCC.Speech;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity.InputModule;
using System.Text;
using UnityEngine.UI;

namespace BM.Unity.BOSCC.Props
{
    /// <summary>
    /// A journal that writes itself!!!
    /// </summary>
    public class NotebookController : MonoBehaviour, IDictationListener, IInputClickHandler
    {
        private StringBuilder textBuffer;
        private Text textDisplay;

        public void OnDictationComplete(DictationCompletionCause cause)
        {
        }

        public void OnDictationError(string error, int result)
        {
        }

        public void OnDictationHypothesis(string text)
        {
            textDisplay.text = textBuffer.ToString() + text + "...";
        }

        public void OnDictationResult(string text)
        {
            textBuffer.Append(text).Append(".");
            textDisplay.text = textBuffer.ToString();

            if (text.ToLower().Contains("end"))
                EndDictation();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            StartDictation();
        }

        private void EndDictation()
        {
            var dictationHandler = DictationHandler.Instance;
            if (dictationHandler != null)
                dictationHandler.EndDictation();
        }

        private void StartDictation()
        {
            var dictationHandler = DictationHandler.Instance;
            if (dictationHandler != null)
            {
                textBuffer = new StringBuilder();
                dictationHandler.SetListener(this);
            }
        }
        // Use this for initialization
        void Start()
        {
            textDisplay = GetComponentInChildren<Text>();
        }
    }
}