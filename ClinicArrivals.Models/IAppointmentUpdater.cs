namespace ClinicArrivals.Models
{
    public interface IAppointmentUpdater
    {
        public void MarkAppointmentAsVideoMeeting(PmsAppointment appointment);
        public void MarkPatientAsArrived(PmsAppointment appointment);

        public void AddViedoConferencingUrl(PmsAppointment appointment, string url);
    }
}