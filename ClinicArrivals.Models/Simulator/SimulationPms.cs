using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicArrivals.Models
{
    [AddINotifyPropertyChangedInterface]
    public class SimulationPms : IFhirAppointmentReader, IFhirAppointmentUpdater
    {
        public ObservableCollection<PmsAppointment> Appointments { get; set; } = new ObservableCollection<PmsAppointment>();
        // practitioner name to FhirID lookup (needed as is used to lookup the room template)
        public Dictionary<string, string> PractitionerFhirIds = new Dictionary<string, string>();

        public PmsAppointment EditingAppointment { get; set; } = new PmsAppointment();
        // The changing of this will just copy the data into the NewA
        public PmsAppointment SelectedAppointment
        {
            get
            {
                return _selectedAppointment;
            }
            set
            {
                if (_selectedAppointment != value)
                {
                    _selectedAppointment = value;
                    if (_selectedAppointment != null)
                    {
                        EditingAppointment.PatientFhirID = _selectedAppointment.PatientFhirID;
                        EditingAppointment.PatientName = _selectedAppointment.PatientName;
                        EditingAppointment.PatientMobilePhone = _selectedAppointment.PatientMobilePhone;
                        EditingAppointment.PractitionerName = _selectedAppointment.PractitionerName;
                        EditingAppointment.PractitionerFhirID = _selectedAppointment.PractitionerFhirID;
                        EditingAppointment.AppointmentFhirID = _selectedAppointment.AppointmentFhirID;
                        EditingAppointment.ArrivalStatus = _selectedAppointment.ArrivalStatus;
                        EditingAppointment.AppointmentStartTime = _selectedAppointment.AppointmentStartTime;
                        EditingAppointment.IsVideoConsultation = _selectedAppointment.IsVideoConsultation;
                    }
                }
            }
        }
        private PmsAppointment _selectedAppointment;

        public ICommand CreateNewAppointment { get; set; }
        public ICommand UpdateAppointment { get; set; }
        public ICommand DeleteAppointment { get; set; }

        public void ExecuteUpdateAppointment()
        {
            // copy the data from the appointment into the list
            if (SelectedAppointment != null)
            {
                SelectedAppointment.PatientName = EditingAppointment.PatientName;
                SelectedAppointment.PatientMobilePhone = EditingAppointment.PatientMobilePhone;
                if (SelectedAppointment.PractitionerName != EditingAppointment.PractitionerName)
                {
                    SelectedAppointment.PractitionerName = EditingAppointment.PractitionerName;
                    if (PractitionerFhirIds.ContainsKey(SelectedAppointment.PractitionerName))
                        SelectedAppointment.PractitionerFhirID = PractitionerFhirIds[SelectedAppointment.PractitionerName];
                    else
                    {
                        SelectedAppointment.PractitionerFhirID = Guid.NewGuid().ToString("X");
                        PractitionerFhirIds.Add(SelectedAppointment.PractitionerName, SelectedAppointment.PractitionerFhirID);
                    }
                }
                SelectedAppointment.ArrivalStatus = EditingAppointment.ArrivalStatus;
                SelectedAppointment.AppointmentStartTime = EditingAppointment.AppointmentStartTime;
                SelectedAppointment.IsVideoConsultation = EditingAppointment.IsVideoConsultation;
            }
        }

        public void ExecuteCreateNewAppointment()
        {
            EditingAppointment.AppointmentFhirID = Guid.NewGuid().ToString("X");
            if (PractitionerFhirIds.ContainsKey(EditingAppointment.PractitionerName))
                EditingAppointment.PractitionerFhirID = PractitionerFhirIds[EditingAppointment.PractitionerName];
            else
            {
                EditingAppointment.PractitionerFhirID = Guid.NewGuid().ToString("X");
                PractitionerFhirIds.Add(EditingAppointment.PractitionerName, EditingAppointment.PractitionerFhirID);
            }

            Appointments.Add(EditingAppointment);
            EditingAppointment = new PmsAppointment();
        }

        #region << IFhirAppointmentReader >>
        public async Task<List<PmsAppointment>> SearchAppointments(DateTime date, IList<DoctorRoomLabelMapping> roomMappings, IArrivalsLocalStorage storage)
        {
            List<PmsAppointment> result = new List<PmsAppointment>();
            result.AddRange(Appointments.Where(a => a.AppointmentStartTime.Date == date.Date));

            foreach (var appt in result)
            {
                // Check if the practitioner has a mapping already
                if (!roomMappings.Any(m => m.PractitionerFhirID == appt.PractitionerFhirID))
                {
                    // Add in an empty room mapping
                    roomMappings.Add(new DoctorRoomLabelMapping()
                    {
                        PractitionerFhirID = appt.PractitionerFhirID,
                        PractitionerName = appt.PractitionerName
                    });
                }
                // And read in the extended content from storage
                await storage.LoadAppointmentStatus(date, appt);
            }

            return result;
        }
        #endregion

        #region << IFhirAppointmentUpdater >>
        public void SaveAppointmentAsVideoMeeting(PmsAppointment appointment, string videoLinkComment)
        {
            // this data doesn't come back, so nothing to actually simulate here
        }

        public void SaveAppointmentStatusValue(PmsAppointment appointment)
        {
            var appt = Appointments.FirstOrDefault(a => a.AppointmentFhirID == appointment.AppointmentFhirID);
            if (appt != null)
            {
                appt.ArrivalStatus = appointment.ArrivalStatus;
            }
        }
        #endregion
    }
}
