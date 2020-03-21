using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ClinicArrivals.Models
{
    public class SmsProcessor : ISmsProcessor
    {
        Settings _settings;
        public void Initialize(Settings settings)
        {
            _settings = settings;
            System.Diagnostics.Debug.WriteLine("account: " + _settings.ACCOUNT_SID);
            System.Diagnostics.Debug.WriteLine("token: " + _settings.AUTH_TOKEN);
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Twilio.TwilioClient.Init(_settings.ACCOUNT_SID, _settings.AUTH_TOKEN);
        }

        /// <summary>
        /// Send a message to the twilio gateway
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(SmsMessage sendMessage)
        {
            // TODO: Work out what options are the most sensible to use here
            var message = MessageResource.Create(
                new PhoneNumber(sendMessage.phone),
                from: new PhoneNumber(_settings.FromTwilioMobileNumber),
                body: sendMessage.message
            );
        }

        /// <summary>
        /// Receive the current messages from the Twilio sms gateway
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<SmsMessage>> ReceiveMessages()
        {
            List<SmsMessage> messages = new List<SmsMessage>();
            string url = "https://test.fhir.org/twilio?AccountSid="+ _settings.ACCOUNT_SID;
            try
            {
                using (var webClient = new HttpClient())
                {
                    var result = await webClient.GetAsync(url);
                    var json = await result.Content.ReadAsStringAsync();
                    var list = JsonConvert.DeserializeObject<List<SmsMessage>>(json);
                    return list;
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
}
