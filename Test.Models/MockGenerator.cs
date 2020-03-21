using ClinicArrivals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.Appointment;

namespace Test.Models
{
    class MockSession
    {
        public List<PmsAppointment> appointments = new List<PmsAppointment>();

        /// <summary>
        ///  This is the mobile phone number of the developer. Any where a destination phone number is used, it will be the developers phone number
        /// </summary>
        public String DeveloperPhoneNumber { get; set; }
        public void Initialize(Settings settings)
        {
            DeveloperPhoneNumber = settings.DeveloperPhoneNumber;
        }

        public void CreateAppointment(int minInFuture)
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient " + (appointments.Count + 1).ToString();
            app.PatientMobilePhone = DeveloperPhoneNumber;
            int id = new Random().Next(1, 8);
            app.PractitionerName = "Dr Adam Ant" + id.ToString();
            app.PractitionerFhirID = "p123-" + id.ToString();
            app.AppointmentFhirID = Guid.NewGuid().ToString();
            app.ArrivalStatus = AppointmentStatus.Pending;
            app.AppointmentStartTime = DateTime.Now.AddMinutes(minInFuture);
            appointments.Add(app);
        }

        public void MarkAppointmentInProgress()
        {
            appointments[appointments.Count - 1].ArrivalStatus = AppointmentStatus.Fulfilled;
        }
        public void CancelAppointment()
        {
            appointments[appointments.Count - 1].ArrivalStatus = AppointmentStatus.Cancelled;
        }
    }
}
