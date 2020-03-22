namespace ClinicArrivals.Models
{
    public interface IFhirAppointmentUpdater
    {
        void MarkAppointmentAsVideoMeeting(PmsAppointment appointment);

        void MarkPatientAsArrived(PmsAppointment appointment);

        void AddViedoConferencingUrl(PmsAppointment appointment, string url);
    }
}