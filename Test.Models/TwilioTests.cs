using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    [TestClass]
    public class TwilioTests
    {
        // retrieve the settings for the unit tests (the appSettings file in the test project)
        Settings GetSettings()
        {
            string json = File.ReadAllText("appSettings.json");
            return JsonConvert.DeserializeObject<Settings>(json);
        }

        [TestMethod, TestCategory("Twilio")]
        public void TestSend()
        {
            SmsProcessor sms = new SmsProcessor();
            sms.Initialize(GetSettings());
            SmsMessage msg = new SmsMessage("+61411867065", "Twilio Test Message");
            sms.SendMessage(msg);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public async Task TestFetch()
        {
            SmsProcessor sms = new SmsProcessor();
            sms.Initialize(GetSettings());
            var messages = await sms.ReceiveMessages();
            foreach (var msg in messages)
            {
                System.Diagnostics.Debug.WriteLine("msg from " + msg.phone + ": " + msg.message);
            }
            Assert.IsTrue(true);
        }
    }
}
