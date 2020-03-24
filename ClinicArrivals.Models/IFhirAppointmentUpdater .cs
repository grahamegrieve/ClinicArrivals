using Hl7.Fhir.Model;

namespace ClinicArrivals.Models
{
    public interface IFhirAppointmentUpdater
    {
        /// <summary>
        /// This will retrieve the FHIR resource, set the AppointmentType for teleconsultation, 
        /// include a link for the video link in the comment and update the resource
        /// </summary>
        /// <param name="appointment"></param>
        /// <param name="videoLinkComment"></param>
        void SaveAppointmentAsVideoMeeting(PmsAppointment appointment, string videoLinkComment, string videoUrl);

        /// <summary>
        /// This will retrieve the FHIR appointment from the server, 
        /// set the status value and then update back to the server
        /// </summary>
        /// <param name="appointment"></param>
        void SaveAppointmentStatusValue(PmsAppointment appointment);
    }
}