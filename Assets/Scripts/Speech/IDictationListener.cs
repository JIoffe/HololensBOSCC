using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace BM.Unity.BOSCC.Speech
{
    public interface IDictationListener
    {
        void OnDictationHypothesis(string text);
        void OnDictationResult(string text);
        void OnDictationComplete(DictationCompletionCause text);
        void OnDictationError(string error, int result);
    }
}