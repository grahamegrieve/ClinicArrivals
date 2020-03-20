using System;
using System.Configuration;
using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Models
{
    [TestClass]
    public class FhirAppointmentHandling
    {
        [TestMethod]
        public async System.Threading.Tasks.Task ReadTodaysAppointments()
        {
            ArrivalsModel model = new ArrivalsModel();
            await MessageProcessing.CheckAppointments(model);
        }

        [TestMethod]
        public void MarkAppointmentArrived()
        {
        }

        [TestMethod]
        public void ReadSettings()
        {
            Console.WriteLine($"Poll interval: {ConfigurationManager.AppSettings.Get("PollIntervalSeconds")}");
            Console.WriteLine($"Intro SMS Message: {ConfigurationManager.AppSettings.Get("IntroSmsMessage")}");
            Assert.IsNotNull(ConfigurationManager.AppSettings.Get("PollIntervalSeconds"));
        }
    }
}
