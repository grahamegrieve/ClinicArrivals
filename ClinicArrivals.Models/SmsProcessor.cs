using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ClinicArrivals.Models
{
    public class SmsProcessor
    {
        public void Initialize()
        {
            // TODO: Is this all the settings that are required for intializing Twilio correctly
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Twilio.TwilioClient.Init(
                ConfigurationManager.AppSettings.Get("ACCOUNT_SID"),
                ConfigurationManager.AppSettings.Get("AUTH_TOKEN"));
        }

        /// <summary>
        /// Have this whitelist of numbers for testing
        /// </summary>
        List<String> whitelistedNumbers = new List<string>() {
        };

        /// <summary>
        /// Send a message to the twilio gateway
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(SmsMessage sendMessage)
        {
            // TODO: Work out what options are the most sensible to use here
            var message = MessageResource.Create(
                new PhoneNumber(sendMessage.phone),
                from: new PhoneNumber(ConfigurationManager.AppSettings.Get("FromTwilioMobileNumber")),
                body: sendMessage.message
            );
            Console.WriteLine(message.Sid);
        }

        /// <summary>
        /// Receive the current messages from the Twilio sms gateway
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SmsMessage> ReceiveMessages()
        {
            List<SmsMessage> results = new List<SmsMessage>();
            // create some test data
            results.Add(new SmsMessage("08523138542", "arrived"));
            results.Add(new SmsMessage("0423857505", "Arrived"));
            results.Add(new SmsMessage("0423857505", "Here"));
            results.Add(new SmsMessage("0423857505", "Ok"));
            return results;
        }
    }

    public class SmsMessage
    {
        public SmsMessage(string phone, string message)
        {
            this.phone = phone;
            this.message = message;
        }
        public string phone { get; set; }
        public string message { get; set; }
    }
}
