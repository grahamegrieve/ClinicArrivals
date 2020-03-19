using System;
using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Models
{
    [TestClass]
    public class FhirAppointmentHandling
    {
        [TestMethod]
        public void ReadTodaysAppointments()
        {
            ArrivalsModel model = new ArrivalsModel();
            MessageProcessing.CheckAppointments(model);
        }

        [TestMethod]
        public void MarkAppointmentArrived()
        {

        }
    }
}
