using ClinicArrivals.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    [TestClass]
    public class FhirUpdaterInterfaceTester
    {
        private FhirClient GetFhirClient()
        {
            return new FhirClient("https://sqlonfhir-r4.azurewebsites.net/fhir", false);
        }

        [TestMethod, TestCategory("FHIR"), TestCategory("SkipWhenLiveUnitTesting")]
        public void SaveAppointmentAsVideoMeeting()
        {
            IFhirAppointmentUpdater updater = new FhirAppointmentUpdater(GetFhirClient);
            var server = GetFhirClient();
            var appt = new PmsAppointment()
            {
                AppointmentFhirID = "example",
                ArrivalStatus = Appointment.AppointmentStatus.Arrived
            };
            string notes = "Video Link: blah2";
            updater.SaveAppointmentAsVideoMeeting(appt, notes);

            var apptSaved = server.Read<Appointment>($"{server.Endpoint}Appointment/{appt.AppointmentFhirID}");
            Assert.IsTrue(apptSaved.Comment.Contains(notes), "expected the notes to contain the video link");
        }

        [TestMethod, TestCategory("FHIR"), TestCategory("SkipWhenLiveUnitTesting")]
        public void SaveAppointmentStatusValue()
        {
            IFhirAppointmentUpdater updater = new FhirAppointmentUpdater(GetFhirClient);
            var server = GetFhirClient();
            var appt = new PmsAppointment()
            {
                AppointmentFhirID = "example",
                ArrivalStatus = Appointment.AppointmentStatus.Arrived
            };
            updater.SaveAppointmentStatusValue(appt);

            var apptSaved = server.Read<Appointment>($"{server.Endpoint}Appointment/{appt.AppointmentFhirID}");
            Assert.AreEqual(Appointment.AppointmentStatus.Arrived, apptSaved.Status);
        }
    }
}
