using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Unity.BOSCC.Bot.Framework.Model
{
    [System.Serializable]
    public class BFNewConversation
    {
        public string conversationId;
        public string token;
        public int expires_in;
        public string streamUrl;
    }
}