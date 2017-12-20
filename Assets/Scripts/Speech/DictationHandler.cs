using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace BM.Unity.BOSCC.Speech
{
    public class DictationHandler : Singleton<DictationHandler>
    {
        [Tooltip("The time in seconds that the dictation engine will wait for the user to begin speaking before timing out.")]
        public float initialDictationSilenceTimeout = 5.0f;

        [Tooltip("The timeout in seconds before ending the dictation stream.")]
        public float dictationSilenceTimeout = 15.0f;

        private IDictationListener DictationListener { get; set; }
        private DictationRecognizer DictationRecognizer { get; set; }

        void Start()
        {
            DictationRecognizer = new DictationRecognizer();

            DictationRecognizer.InitialSilenceTimeoutSeconds = initialDictationSilenceTimeout;
            DictationRecognizer.AutoSilenceTimeoutSeconds = dictationSilenceTimeout;

            DictationRecognizer.DictationHypothesis += DictationRecognizer_DictationHypothesis;
            DictationRecognizer.DictationResult += DictationRecognizer_DictationResult;
            DictationRecognizer.DictationComplete += DictationRecognizer_DictationComplete;
            DictationRecognizer.DictationError += DictationRecognizer_DictationError;
        }

        public void SetListener(IDictationListener dictationLister)
        {
            if (DictationRecognizer.Status == SpeechSystemStatus.Running)
                return;

            ShutdownPhraseRecognitionSystem();
            DictationRecognizer.Start();
            DictationListener = dictationLister;
        }

        public void EndDictation()
        {
            if (DictationRecognizer.Status == SpeechSystemStatus.Running)
                DictationRecognizer.Stop();

            DictationListener = null;
            RestartPhraseRecognitionSystem();
        }

        override protected void OnDestroy()
        {
            EndDictation();
            DictationRecognizer.Dispose();
        }

        private void ShutdownPhraseRecognitionSystem()
        {
            if (PhraseRecognitionSystem.Status != SpeechSystemStatus.Running)
                return;

            PhraseRecognitionSystem.Shutdown();
        }

        private void RestartPhraseRecognitionSystem()
        {
            if (PhraseRecognitionSystem.Status == SpeechSystemStatus.Running)
                return;

            PhraseRecognitionSystem.Restart();
        }

        private void DictationRecognizer_DictationHypothesis(string text)
        {
            if(DictationListener != null)
            {
                DictationListener.OnDictationHypothesis(text);
            }
        }

        private void DictationRecognizer_DictationResult(string text, ConfidenceLevel confidence)
        {
            if(DictationListener != null)
            {
                DictationListener.OnDictationResult(text);
            }
        }

        private void DictationRecognizer_DictationComplete(DictationCompletionCause cause)
        {
            if(DictationListener != null)
            {
                DictationListener.OnDictationComplete(cause);
            }
            EndDictation();
        }

        private void DictationRecognizer_DictationError(string error, int result)
        {
            if(DictationListener != null)
            {
                DictationListener.OnDictationError(error, result);
            }
            EndDictation();
        }
    }
}