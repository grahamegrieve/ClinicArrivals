using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Linq;

namespace ClinicArrivals.Models
{
    public class FhirAppointmentUpdater : IFhirAppointmentUpdater
    {
        public FhirAppointmentUpdater(Func<FhirClient> GetFhirClient)
        {
            this.GetFhirClient = GetFhirClient;
        }
        private Func<FhirClient> GetFhirClient;

        /// <summary>
        /// This will retrieve the FHIR resource, set the AppointmentType for teleconsultation, 
        /// include a link for the video link in the comment and update the resource
        /// </summary>
        /// <param name="appointment"></param>
        /// <param name="videoLinkComment"></param>
        public void SaveAppointmentAsVideoMeeting(PmsAppointment appt, string videoLinkComment)
        {
            // Get the Appointment based on the appointment having an ID
            var fhirServer = GetFhirClient();
            Hl7.Fhir.Model.Appointment fhirAppt = fhirServer.Read<Appointment>($"{fhirServer.Endpoint}Appointment/{appt.AppointmentFhirID}");

            CodeableConcept teleHealth = new CodeableConcept("http://hl7.org/au/fhir/CodeSystem/AppointmentType", "teleconsultation");
            if (fhirAppt.AppointmentType.Coding.FirstOrDefault()?.System != teleHealth.Coding[0].System
                || fhirAppt.AppointmentType.Coding.FirstOrDefault()?.Code != teleHealth.Coding[0].Code
                || !fhirAppt.Comment.Contains(videoLinkComment))
            {
                fhirAppt.AppointmentType = teleHealth;
                fhirAppt.Comment = String.IsNullOrEmpty(fhirAppt.Comment) ?
                   videoLinkComment
                   : fhirAppt.Comment + Environment.NewLine + Environment.NewLine + videoLinkComment;
                fhirServer.Update(fhirAppt);
            }
        }

        /// <summary>
        /// This will retrieve the FHIR appointment from the server, 
        /// set the status value and then update back to the server
        /// </summary>
        /// <param name="appointment"></param>
        public void SaveAppointmentStatusValue(PmsAppointment appt)
        {
            // Get the Appointment based on the appointment having an ID 
            // and update the status value
            var fhirServer = GetFhirClient();
            Hl7.Fhir.Model.Appointment fhirAppt = fhirServer.Read<Appointment>($"{fhirServer.Endpoint}Appointment/{appt.AppointmentFhirID}");
            if (fhirAppt.Status != appt.ArrivalStatus)
            {
                // Don't save it if hasn't changed
                fhirAppt.Status = appt.ArrivalStatus;
                fhirServer.Update(fhirAppt);
            }
        }
    }
}
