using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;

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
            Hl7.Fhir.Model.Appointment fhirAppt = fhirServer.Read<Appointment>(appt.AppointmentFhirID);
            fhirAppt.AppointmentType = new CodeableConcept("http://hl7.org/au/fhir/CodeSystem/AppointmentType", "teleconsultation");
            fhirAppt.Comment = String.IsNullOrEmpty(fhirAppt.Comment) ?
               videoLinkComment
               : fhirAppt.Comment + Environment.NewLine + Environment.NewLine + videoLinkComment;
            fhirServer.Update(fhirAppt);
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
            Hl7.Fhir.Model.Appointment fhirAppt = fhirServer.Read<Appointment>(appt.AppointmentFhirID);
            fhirAppt.Status = appt.ArrivalStatus;
            fhirServer.Update(fhirAppt);
        }
    }
}
