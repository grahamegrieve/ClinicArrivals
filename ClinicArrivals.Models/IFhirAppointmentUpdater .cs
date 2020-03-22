using Hl7.Fhir.Model;

namespace ClinicArrivals.Models
{
    public interface IFhirAppointmentUpdater
    {
        /// <summary>
        ///   Fetch the appointment prior to using one of the put methods below
        /// </summary>
        /// <param name="id">the id of the resource </param>
        /// <returns></returns>
        Appointment fetch(string id);

        void PutAsVideoMeeting(Appointment appointment);

        void PutStatusArrived(Appointment appointment);

        void PutVideoUrl(Appointment appointment);
    }
}