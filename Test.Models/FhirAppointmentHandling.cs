using System;
using System.Configuration;
using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Models
{
    [TestClass]
    public class FhirAppointmentHandling
    {
        [TestInitialize]
        public void Initialize()
        {
            // Start the FHIR Server
            MessageProcessing.StartServer(true);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task ReadTodaysAppointments()
        {
            ArrivalsModel model = new ArrivalsModel();
            await MessageProcessing.CheckAppointments(model);
        }

        [TestMethod]
        public async System.Threading.Tasks.Task MarkAppointmentArrived()
        {
            ArrivalsModel model = new ArrivalsModel();

            while( !MessageProcessing.IsRunning() )
            {
                System.Threading.Thread.Sleep(500);
            }

            await MessageProcessing.CheckAppointments(model);

            var e = model.Expecting.GetEnumerator();
            e.MoveNext();
            PmsAppointment appt = e.Current ;
            var result = MessageProcessing.ArriveAppointment(appt);
        }

        [TestMethod]
        public void MarkAppointmentStarted()
        {
        }
    }
}
