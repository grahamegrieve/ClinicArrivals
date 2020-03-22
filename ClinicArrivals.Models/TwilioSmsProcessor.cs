using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace ClinicArrivals.Models
{
    public class TwilioSmsProcessor : ISmsProcessor
    {
        Settings _settings;
        public void Initialize(Settings settings)
        {
            _settings = settings;
            System.Diagnostics.Debug.WriteLine("account: " + _settings.ACCOUNT_SID);
            System.Diagnostics.Debug.WriteLine("token: " + _settings.AUTH_TOKEN);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        /// <summary>
        /// Send a message to the Twilio gateway
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(SmsMessage sendMessage)
        {
            String url = "https://api.twilio.com/2010-04-01/Accounts/" + _settings.ACCOUNT_SID + "/Messages";
            String body = "To="+sendMessage.phone+"&From="+ _settings.FromTwilioMobileNumber + "&Body="+ Uri.EscapeDataString(sendMessage.message);
            var webRequest = WebRequest.Create(url);
            webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(_settings.ACCOUNT_SID + ":" + _settings.AUTH_TOKEN));
            webRequest.Method = "POST";
            var bytes = Encoding.UTF8.GetBytes(body);
            webRequest.ContentLength = bytes.Length;
            webRequest.ContentType = "application/x-www-form-urlencoded";
            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            using (HttpWebResponse response = (HttpWebResponse) webRequest.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.Created)
                {
                    throw new Exception("Twilio post failed");
                }
            }
        }

        private CredentialCache GetCredential(String url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new System.Uri(url), "Basic", new NetworkCredential(_settings.ACCOUNT_SID, _settings.AUTH_TOKEN));
            return credentialCache;
        }

        /// <summary>
        /// Receive the current messages from the Twilio sms gateway
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SmsMessage>> ReceiveMessages()
        {
            List<SmsMessage> messages = new List<SmsMessage>();
            string url = "https://clinics.healthintersections.com.au/twilio?AccountSid="+ _settings.ACCOUNT_SID;
            try
            {
                using (var webClient = new HttpClient())
                {
                    var result = await webClient.GetAsync(url);
                    var json = await result.Content.ReadAsStringAsync();
                    var msgs = JsonConvert.DeserializeObject<SmsMessageResponse>(json);
                    return msgs.messages;
                }
            }
            catch (Exception ex)
            {
                // not sure what to do
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return null;
            }
        }
    }

    public class SmsMessageResponse
    {
        public List<SmsMessage> messages  { get; set; }

    }
}
