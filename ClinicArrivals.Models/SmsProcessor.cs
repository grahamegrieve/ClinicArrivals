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
        public void Initialize()
        {
            System.Diagnostics.Debug.WriteLine("account: " + ConfigurationManager.AppSettings.Get("ACCOUNT_SID"));
            System.Diagnostics.Debug.WriteLine("token: " + ConfigurationManager.AppSettings.Get("AUTH_TOKEN"));
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Twilio.TwilioClient.Init(
                ConfigurationManager.AppSettings.Get("ACCOUNT_SID"),
                ConfigurationManager.AppSettings.Get("AUTH_TOKEN"));
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
                from: new PhoneNumber(ConfigurationManager.AppSettings.Get("SEND_NUMBER")),
                body: sendMessage.message
            );
        }

        /// <summary>
        /// Receive the current messages from the Twilio sms gateway
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SmsMessage> ReceiveMessages()
        {
            List<SmsMessage> messages = new List<SmsMessage>();
            string url = "http://test.fhir.org/twilio?AccountSid="+ ConfigurationManager.AppSettings.Get("ACCOUNT_SID");
            try
            {
                using (var webClient = new System.Net.WebClient())
                {
                    var json = webClient.DownloadString(url);
                    var list = JsonConvert.DeserializeObject<List<SmsMessage>>(json);
                    return list;
                }
            }
            catch (Exception exception)
            {
                // not sure what to do
                return null;
            }
      
                List<SmsMessage> results = new List<SmsMessage>();
            // create some test data
            results.Add(new SmsMessage("08523138542", "arrived"));
            results.Add(new SmsMessage("0423857505", "Arrived"));
            results.Add(new SmsMessage("0423857505", "Here"));
            results.Add(new SmsMessage("0423857505", "Ok"));
            return results;
        }
    }

   
}
