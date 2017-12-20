using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Unity.BOSCC.Bot.Framework.Model
{
    [System.Serializable]
    public class BFActivityHistory
    {
        public BFActivity[] activities;
        public string watermark;

        public List<BFActivity> GetMessagesByUser(string userId)
        {
            var userActivities = new List<BFActivity>();

            foreach(var activity in activities)
            {
                if (!activity.type.Equals("message"))
                    continue;

                if (activity.from.id.Equals(userId))
                    userActivities.Add(activity);
            }

            return userActivities;
        }
    }
}