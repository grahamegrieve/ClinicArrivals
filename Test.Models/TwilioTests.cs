using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    [TestClass]
    public class TwilioTests
    {
        [TestMethod]
        public void TestSend()
        {
            SmsProcessor sms = new SmsProcessor();
            sms.Initialize();
            SmsMessage msg = new SmsMessage("+61411867065", "Twilio Test Message");
            sms.SendMessage(msg);
            Assert.IsTrue(true);
        }


        [TestMethod]
        public void TestFetch()
        {
            SmsProcessor sms = new SmsProcessor();
            sms.Initialize();
            foreach (var msg in sms.ReceiveMessages())
            {
                System.Diagnostics.Debug.WriteLine("msg from "+msg.phone+": "+msg.message);
            }
            Assert.IsTrue(true);
        }

    }
}
