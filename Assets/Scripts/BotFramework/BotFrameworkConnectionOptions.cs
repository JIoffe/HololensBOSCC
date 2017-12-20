using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Unity.BOSCC.Bot.Framework
{
    [System.Serializable]
    public class BotFrameworkConnectionOptions
    {
        [System.Serializable]
        public class Bot
        {
            [Tooltip("The name of the bot within the conversation logs")]
            public string botName;

            [Tooltip("The Bot API secret provided through the DirectLine channel")]
            public string botSecret;
        }
        [System.Serializable]
        public class Config
        {
            [Tooltip("The root URL for the bot framework direct line API")]
            public string rootUrl = "https://directline.botframework.com";

            [Tooltip("The endpoint for creating a new conversation")]
            public string newConvoEndpoint = "/v3/directline/conversations";

            [Tooltip("The endpoint to get/post messages to the active conversation")]
            public string conversationEndpoint = "/v3/directline/conversations/{conversationId}/activities";

            [Tooltip("The time in seconds between each call to the API to listen for new dialogue from the bot")]
            public float conversationPollingFrequency = 0.25f;
        }

        public Bot bot;
        public Config config;
    }
}