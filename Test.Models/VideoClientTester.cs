using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    [TestClass, Ignore] // you can only run this if you provide the correct API token and since the only place to do this is in the checked in code, we don't usually run these tests 
    public class VideoClientTester
    {
        private string secret = "";

        [TestMethod, TestCategory("SkipWhenLiveUnitTesting")]
        public void testCreateSession()
        {
            var client = new OpenViduClient("https://video.healthintersections.com.au:4443", secret);
            Assert.IsNotNull(client.SetUpSession());

        }

        [TestMethod, TestCategory("SkipWhenLiveUnitTesting")]
        public void testHasMemberNo()
        {
            var client = new OpenViduClient("https://video.healthintersections.com.au:4443", secret);
            Assert.IsFalse(client.hasAnyoneJoined("ses_Gu28CIUGkt"));

        }


        [TestMethod, TestCategory("SkipWhenLiveUnitTesting")]
        public void testHasMemberYes()
        {
            var client = new OpenViduClient("https://video.healthintersections.com.au:4443", secret);
            Assert.IsTrue(client.hasAnyoneJoined("ses_A3wAzHMfaZ"));

        }
    }
}
