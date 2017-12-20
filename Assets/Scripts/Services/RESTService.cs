using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BM.Unity.BOSCC.Services.Net
{
    /// <summary>
    /// Supports calling endpoints on the internet and parsing JSON. These are implemented using
    /// coroutines in Unity due to the lack of newer C# Task functionalities
    /// </summary>
    public class RESTService : Singleton<RESTService>
    {
        public WWW Get<T>(string url, Dictionary<string, string> headerFields, System.Action<T> callback)
        {
            WWW www;
            if (headerFields != null && headerFields.Count > 0)
                www = new WWW(url, null, headerFields);
            else
                www = new WWW(url);

            StartCoroutine(_SendRequest(www, callback));

            return www;
        }

        public WWW Get<T>(string url, System.Action<T> callback)
        {
            return Get(url, null, callback);
        }

        //Posts that expect JSON to be returned
        public WWW Post<T>(string url, object parameters, Dictionary<string, string> headers, System.Action<T> callback)
        {
            var bodyData = _GetParamData(parameters);

            WWW www = new WWW(url, bodyData, headers);

            //Always run in parallel to not block the Unity main thread
            StartCoroutine(_SendRequest(www, callback));
            return www;
        }
        public WWW Post<T>(string url, object parameters, System.Action<T> callback)
        {
            return Post(url, parameters, null, callback);
        }

        public WWW Post<T>(string url, System.Action<T> callback)
        {
            return Post(url, null, null, callback);
        }

        //POST requests that do not anticipate any data 
        public WWW Post(string url, object parameters, Dictionary<string, string> headers, System.Action callback)
        {
            var bodyData = _GetParamData(parameters);

            WWW www = new WWW(url, bodyData, headers);

            //Always run in parallel to not block the Unity main thread
            StartCoroutine(_SendRequest(www, callback));
            return www;
        }

        public WWW Post(string url, object parameters, System.Action callback)
        {
            return Post(url, parameters, null, callback);
        }

        public WWW Post(string url, System.Action callback)
        {
            return Post(url, null, null, callback);
        }

        private IEnumerator _SendRequest<T>(WWW www, System.Action<T> callback)
        {
            yield return www;

            //Debug.Log("Got response back from server - " + www.url);
            if (www.error != null)
            {
                Debug.LogError("Could not access " + www.url);
                Debug.LogError(www.error);
            }
            else
            {
                if (callback != null)
                {
                    var results = _ParseJSON<T>(www.text);
                    callback(results);
                }
            }


        }

        /// <summary>
        /// Send a request that does not expect any data in return
        /// </summary>
        /// <param name="www"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        private IEnumerator _SendRequest(WWW www, System.Action callback)
        {
            yield return www;

            //Debug.Log("Got response back from server - " + www.url);
            if (www.error != null)
            {
                Debug.LogError("Could not access " + www.url);
                Debug.LogError(www.error);
            }
            else
            {
                if (callback != null)
                    callback();
            }
        }

        private byte[] _GetParamData(object o)
        {
            if (o == null)
                return EmptyData;

            if (o is Dictionary<string, object>)
            {
                var parameters = (Dictionary<string, string>)o;
                if (parameters.Count == 0)
                    return EmptyData;

                //Let the WWWForm wrap these parameter values
                WWWForm form = new WWWForm();

                foreach (var param in parameters)
                {
                    form.AddField(param.Key, param.Value.ToString());
                }

                return form.data;
            }

            //Otherwise, we must be using JSON
            return _GetBytes(o);
        }

        private byte[] EmptyData
        {
            get
            {
                return new byte[] { 0 };
            }
        }
        private byte[] _GetBytes(object o)
        {
            var json = _ToJSON(o);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        private T _ParseJSON<T>(string text)
        {
            return JsonUtility.FromJson<T>(text);
        }

        private string _ToJSON(object src)
        {
            return JsonUtility.ToJson(src);
        }
    }
}