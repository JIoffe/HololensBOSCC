using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BM.Unity.BOSCC.Speech;
using UnityEngine.Windows.Speech;
using BM.Unity.BOSCC.Bot.Framework;
using BM.Unity.BOSCC.GUI;
using HoloToolkit.Unity;
using System.Text.RegularExpressions;

namespace BM.Unity.BOSCC.Actors
{
    /// <summary>
    /// Bare-bones controller for a digital assistant 
    /// </summary>
    [RequireComponent(typeof(BotConnector))]
    public class ChatBotController : MonoBehaviour, IInputClickHandler, IDictationListener
    {
        private static readonly Regex multipleSpaces = new Regex(@"\s+");
        private Animator animator;
        private Moodiness moodiness;
        private BotConnector botConnector;
        private SpeechBubble speechBubble;
        private TextToSpeechManager ttsManager;

        void Start()
        {
            animator = GetComponentInChildren<Animator>();
            moodiness = GetComponentInChildren<Moodiness>();
            botConnector = GetComponentInChildren<BotConnector>();
            speechBubble = GetComponentInChildren<SpeechBubble>();
            ttsManager = GetComponentInChildren<TextToSpeechManager>();

            botConnector.OnBotResponse += OnBotResponse;
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            StartConversation();
        }

        private void StartConversation()
        {
            if (botConnector.InConversation)
            {
                return;
            }

            Wave();
            Echo("Oh! A human.");
            botConnector.StartConversation();
            StartDictation();
        }

        private void StopConversation()
        {
            botConnector.StopConversation();
            EndDictation();
        }
        private void Wave()
        {
            moodiness.ChangeMood(Mood.Excited);
            animator.SetTrigger("Wave");
        }

        //Conversation Callbacks
        private void OnBotResponse(string response)
        {
            Echo(response);
        }

        public void OnDictationHypothesis(string text)
        {
            moodiness.ChangeMood(Mood.Happy);
        }

        public void OnDictationResult(string text)
        {
            if (ResponseIsRude(text))
            {
                moodiness.ChangeMood(Mood.Sad);
                Echo("Don't be a bully", "Sulk");
                return;
            }

            if (ResponseIsApologetic(text))
            {
                Echo("It's ok, we're still friends");
                Wave();
                return;
            }

            if (text.ToLowerInvariant().Contains("bye"))
            {
                Echo("Have a nice day!", "Wave");
                StopConversation();
                return;
            }

            if (botConnector.InConversation)
                botConnector.DispatchUserMessage(text);

            moodiness.ChangeMood(Mood.Excited);
        }

        public void OnDictationComplete(DictationCompletionCause text)
        {
            Echo("Speechless? Alright, talk to you later!", "Wave");
            StopConversation();
        }

        public void OnDictationError(string error, int result)
        {
            Echo("Something is wrong with your mic, talk later!", "Wave");
            StopConversation();
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
                dictationHandler.SetListener(this);
            }
        }

        private bool ResponseIsRude(string text)
        {
            var ltext = text.ToLowerInvariant();

            return ltext.Contains("stupid") || ltext.Contains("dumb") || ltext.Contains("idiot");
        }

        private bool ResponseIsApologetic(string text)
        {
            var ltext = text.ToLowerInvariant();
            return ltext.Contains("sorry") || ltext.Contains("apologize");
        }

        private void Echo(string s, string animation = "Emphasize")
        {
            //Prune text before speech
            s = multipleSpaces.Replace(s, " ");

            if (speechBubble != null)
                speechBubble.Echo(s);

            if (ttsManager != null)
                ttsManager.SpeakText(s);

            moodiness.ClearMood();
            animator.SetTrigger(animation);
        }
    }
}