using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace ClinicArrivals.Models
{
    public class OpenViduClient
    {
        private string BaseUrl;

        public string Secret { get; }

        public OpenViduClient(string baseUrl, string secret)
        {
            this.BaseUrl = baseUrl;
            this.Secret = secret;
        }

        public string SetUpSession()
        {
            // https://openvidu.io/docs/reference-docs/REST-API/#post-apisessions

            String url = BaseUrl + "/api/sessions";
            String body = "{\"mediaMode\":\"ROUTED\"}";
            var webRequest = WebRequest.Create(url);
            webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("OPENVIDUAPP:" + Secret));
            webRequest.Method = "POST";
            var bytes = Encoding.UTF8.GetBytes(body);
            webRequest.ContentLength = bytes.Length;
            webRequest.ContentType = "application/json";
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception("Video server post failed");
                }
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string text = reader.ReadToEnd();
                    var resp = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateSessionResponse>(text);
                    return resp.id;
                }
            }
        }

        public bool hasAnyoneJoined(string sessionId)
        {

            // https://openvidu.io/docs/reference-docs/REST-API/#get-apisessionsltsession_idgt

            String url = BaseUrl + "/api/sessions/"+sessionId;
            String body = "{\"mediaMode\":\"ROUTED\"}";
            var webRequest = WebRequest.Create(url);
            webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("OPENVIDUAPP:" + Secret));
            webRequest.Method = "GET";
            var bytes = Encoding.UTF8.GetBytes(body);
            using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return false;
                }
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string text = reader.ReadToEnd();
                    var resp = Newtonsoft.Json.JsonConvert.DeserializeObject<GetSessionResponse>(text);
                    return resp.connections.numberOfElements > 0;
                }
            }
        }
    }

    internal class CreateSessionResponse
    {
        public string id { get; set; }
    }

    internal class GetSessionResponse
    {
        public string sessionId { get; set; }
        public string recording { get; set; }

        public GetSessionResponseConnection connections { get; set; }
    }

    internal class GetSessionResponseConnection
    {
        public int numberOfElements { get; set; }
    }
}