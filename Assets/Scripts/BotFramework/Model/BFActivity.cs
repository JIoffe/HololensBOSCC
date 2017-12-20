using System;

namespace BM.Unity.BOSCC.Bot.Framework.Model
{
    [Serializable]
    public class BFActivity
    {
        public BFActivity(string user, string messageText)
        {
            type = "message";
            channelId = "directline";
            from = new BFFrom
            {
                id = user
            };
            text = messageText;
        }

        public string type;
        public string id;
        public DateTime timestamp;
        public string channelId;
        public BFFrom from;
        public BFConversation conversation;
        public string text;
        public string replyToId;
    }
}