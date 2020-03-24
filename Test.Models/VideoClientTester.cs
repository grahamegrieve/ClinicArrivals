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
    public class VideoClientTester
    {
        private string secret = "";

        [TestMethod]
        public void testCreateSession()
        {
            var client = new OpenViduClient("https://video.healthintersections.com.au:4443", secret);
            Assert.IsNotNull(client.SetUpSession());

        }

        [TestMethod]
        public void testHasMemberNo()
        {
            var client = new OpenViduClient("https://video.healthintersections.com.au:4443", secret);
            Assert.IsFalse(client.hasAnyoneJoined("ses_Gu28CIUGkt"));

        }


        [TestMethod]
        public void testHasMemberYes()
        {
            var client = new OpenViduClient("https://video.healthintersections.com.au:4443", secret);
            Assert.IsTrue(client.hasAnyoneJoined("ses_A3wAzHMfaZ"));

        }
    }
}
