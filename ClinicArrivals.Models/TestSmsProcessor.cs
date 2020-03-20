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
    public class TestSmsProcessor : ISmsProcessor
    {
        public void Initialize()
        {
        }

        /// <summary>
        /// Send a message to the twilio gateway
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(SmsMessage sendMessage)
        {
            System.Diagnostics.Trace.WriteLine($"SendMessage: {sendMessage.phone} - {sendMessage.message}");
        }

        /// <summary>
        /// Receive the current messages from the Twilio sms gateway
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SmsMessage> ReceiveMessages()
        {
            List<SmsMessage> results = new List<SmsMessage>();
            // create some test data
            // TODO: Read these from a test data file
            results.Add(new SmsMessage("08523138542", "arrived"));
            results.Add(new SmsMessage("0423857505", "Arrived"));
            results.Add(new SmsMessage("0423857505", "Here"));
            results.Add(new SmsMessage("0423857505", "Ok"));
            return results;
        }
    }
}
