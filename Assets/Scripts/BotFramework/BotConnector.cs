using BM.Unity.BOSCC.Bot.Framework.Model;
using BM.Unity.BOSCC.Services.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Unity.BOSCC.Bot.Framework
{
    public delegate void ResponseCallbackDelegate(string response);
    public delegate void ConversationStartCallbackDelegate(BFNewConversation result);

    /// <summary>
    /// Reperesents an active conversation with a chatbot via the MS Bot Framework
    /// </summary>
    public class BotConnector: MonoBehaviour
    {
        [Flags]
        private enum ConversationState
        {
            Stopped = 0,
            Started = 0x01,
            IsPolling = 0x02
        }

        [Tooltip("Bot specific and app-wide settings for Bot Framework connections")]
        public BotFrameworkConnectionOptions connectionOptions;

        //Delegates
        public ResponseCallbackDelegate OnBotResponse { get; set; }
        public ConversationStartCallbackDelegate onConversationStart { get; set; }
        
        public bool IsPolling
        {
            get
            {
                return (State & ConversationState.IsPolling) != 0;
            }
        }

        public bool InConversation
        {
            get
            {
                return (State & ConversationState.Started) != 0;
            }
        }
        private ConversationState State { get; set; }
        private string ConversationId { get; set; }
        private string LastWatermark { get; set; }

        public void PostUserResponse(string response)
        {

        }


        public void StartConversation()
        {
            if (InConversation)
            {
                return;
            }

            State |= ConversationState.Started;

            var header = GenerateHeader();
            var url = connectionOptions.config.rootUrl + connectionOptions.config.newConvoEndpoint;

            RESTService.Instance.Post<BFNewConversation>(url, null, header, ConversationStartCallback);
        }

        public void StopConversation()
        {
            State = ConversationState.Stopped;
        }

        ///////////////////
        /// POLLING
        ///////////////////
        private void StartPolling()
        {
            State |= ConversationState.IsPolling;
            StartCoroutine(PollNewMessages());
        }
        private void StopPolling()
        {
            State &= ~ConversationState.IsPolling;
        }
        private IEnumerator PollNewMessages()
        {
            while (IsPolling)
            {
                var header = GenerateHeader();
                var url = connectionOptions.config.rootUrl + connectionOptions.config.conversationEndpoint.Replace("{conversationId}", ConversationId);

                //Only get new messages
                url += "?watermark=" + LastWatermark;
                RESTService.Instance.Get<BFActivityHistory>(url, header, BotResponseCallback);
                yield return new WaitForSeconds(connectionOptions.config.conversationPollingFrequency);
            }
        }

        /*
         * CALLBACKS
         */
        private void UserResponseSentCallback()
        {
            //Invoked when a user response has been accepted by the bot framework
        }
        private void ConversationStartCallback(BFNewConversation result)
        {
            if(onConversationStart != null)
                onConversationStart.Invoke(result);

            Debug.Log("Started Conversation: " + result.conversationId);
            ConversationId = result.conversationId;
            StartPolling();
        }
        private void BotResponseCallback(BFActivityHistory activityHistory)
        {
            if (activityHistory.activities == null || activityHistory.activities.Length == 0)
                return;

            LastWatermark = activityHistory.watermark;

            var botResponses = activityHistory.GetMessagesByUser(connectionOptions.bot.botName);

            if (OnBotResponse != null)
            {
                foreach (BFActivity response in botResponses)
                {
                    OnBotResponse.Invoke(response.text);
                }
            }
        }

        ////////////////////
        ///DISPATCHING
        /////////////////////
        public void DispatchUserMessage(string text)
        {
            var activity = new BFActivity("UnityTest", text);
            var header = GenerateHeader();
            var url = connectionOptions.config.rootUrl + connectionOptions.config.conversationEndpoint.Replace("{conversationId}", ConversationId);

            RESTService.Instance.Post(url, activity, header, UserResponseSentCallback);
        }

        //Generate consistent HTTP header for Direct Line API
        private Dictionary<string, string> GenerateHeader()
        {
            var headers = new Dictionary<string, string>();

            //The following header must be present in all requests to the DirectLine API
            headers["Authorization"] = "Bearer " + connectionOptions.bot.botSecret;
            headers["Content-Type"] = "application/json";
            return headers;
        }

    }
}
