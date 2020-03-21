using Newtonsoft.Json;
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
    public interface ISmsProcessor
    {
        void Initialize(Settings settings);

        /// <summary>
        /// Send a message to the twilio gateway
        /// </summary>
        /// <param name="message"></param>
        void SendMessage(SmsMessage sendMessage);

        /// <summary>
        /// Receive the current messages from the Twilio sms gateway
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SmsMessage>> ReceiveMessages();
    }

    public class SmsMessage
    {
        public SmsMessage(string phone, string message)
        {
            this.phone = phone;
            this.message = message;
        }
        [JsonProperty("from")]
        public string phone { get; set; }

        [JsonProperty("body")]
        public string message { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }
    }
}
