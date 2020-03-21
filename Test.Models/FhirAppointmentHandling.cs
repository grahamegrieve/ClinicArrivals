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
        public async System.Threading.Tasks.Task MarkAppointmentBooked()
        {
            await new MessageProcessing().ArriveAppointment(null);
        }

        [TestMethod]
        public void MarkAppointmentArrived()
        {
        }

        [TestMethod]
        public void MarkAppointmentStarted()
        {
        }
    }
}
