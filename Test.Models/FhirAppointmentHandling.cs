using System;
using System.Configuration;
using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.Models
{
    [TestClass, Ignore]
    public class FhirAppointmentHandling
    {
        [TestInitialize]
        public void Initialize()
        {
            // Start the FHIR Server
            FhirAppointmentReader.StartServer(true, "", "");
        }

        [TestMethod, Ignore]
        public async System.Threading.Tasks.Task ReadTodaysAppointments()
        {
            ArrivalsModel model = new ArrivalsModel();
            var appts = await new FhirAppointmentReader(FhirAppointmentReader.GetServerConnection).SearchAppointments(model.DisplayingDate, model.RoomMappings, model.Storage);
        }
    }
}
