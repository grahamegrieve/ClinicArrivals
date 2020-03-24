using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static Hl7.Fhir.Model.Appointment;

namespace ClinicArrivals.Models
{
    [AddINotifyPropertyChangedInterface]
    public class SimulationPms : IFhirAppointmentReader, IFhirAppointmentUpdater
    {
        public IArrivalsLocalStorage Storage;
        public ObservableCollection<PmsAppointment> Appointments { get; set; } = new ObservableCollection<PmsAppointment>();
        // practitioner name to FhirID lookup (needed as is used to lookup the room template)
        public ObservableCollection<PractitionerId> PractitionerFhirIds = new ObservableCollection<PractitionerId>();


        public async void Initialize(IArrivalsLocalStorage storage)
        {
            this.Storage = storage;
            Appointments.Clear();
            foreach (var map in await storage.LoadSimulationAppointments())
                Appointments.Add(map);
            PractitionerFhirIds.Clear();
            foreach (var id in await storage.LoadSimulationIds())
                PractitionerFhirIds.Add(id);

            DateTime dt = DateTime.Now;
            EditingAppointment.AppointmentStartTime = dt.AddTicks(-(dt.Ticks % TimeSpan.TicksPerSecond));
            EditingAppointment.ArrivalStatus = AppointmentStatus.Booked;
        }


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
                SelectedAppointment.PractitionerName = EditingAppointment.PractitionerName;
                SelectedAppointment.PractitionerFhirID = null;
                foreach (var pid in PractitionerFhirIds)
                {
                    if (pid.Name == SelectedAppointment.PractitionerName)
                    {
                        SelectedAppointment.PractitionerFhirID = pid.Id;
                    }
                }
                if (SelectedAppointment.PractitionerFhirID == null)
                {
                    String id = Guid.NewGuid().ToString("X");
                    PractitionerFhirIds.Add(new PractitionerId(SelectedAppointment.PractitionerName, id));
                    SelectedAppointment.PractitionerFhirID = id;
                }
                SelectedAppointment.ArrivalStatus = EditingAppointment.ArrivalStatus;
                SelectedAppointment.AppointmentStartTime = EditingAppointment.AppointmentStartTime;
                SelectedAppointment.IsVideoConsultation = EditingAppointment.IsVideoConsultation;
                Storage.SaveSimulationAppointments(Appointments);
                Storage.SaveSimulationIds(PractitionerFhirIds);
            }
        }

        public void ExecuteCreateNewAppointment()
        {
            EditingAppointment.AppointmentFhirID = Guid.NewGuid().ToString("X");

            EditingAppointment.PractitionerFhirID = null;
            foreach (var pid in PractitionerFhirIds)
            {
                if (pid.Name == EditingAppointment.PractitionerName)
                {
                    EditingAppointment.PractitionerFhirID = pid.Id;
                }
            }
            if (EditingAppointment.PractitionerFhirID == null)
            {
                String id = Guid.NewGuid().ToString("X");
                PractitionerFhirIds.Add(new PractitionerId(EditingAppointment.PractitionerName, id));
                EditingAppointment.PractitionerFhirID = id;
            }

            Appointments.Add(EditingAppointment);
            Storage.SaveSimulationAppointments(Appointments);
            Storage.SaveSimulationIds(PractitionerFhirIds);
            EditingAppointment = new PmsAppointment();
            DateTime dt = DateTime.Now;
            EditingAppointment.AppointmentStartTime = dt.AddTicks(-(dt.Ticks % TimeSpan.TicksPerSecond));
            EditingAppointment.ArrivalStatus = AppointmentStatus.Booked;
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
                await storage.LoadAppointmentStatus(appt);
            }

            return result;
        }
        #endregion

        #region << IFhirAppointmentUpdater >>
        public void SaveAppointmentAsVideoMeeting(PmsAppointment appointment, string videoLinkComment, string VideoUrl)
        {
            var appt = Appointments.FirstOrDefault(a => a.AppointmentFhirID == appointment.AppointmentFhirID);
            if (appt != null)
            {
                appt.IsVideoConsultation = appointment.IsVideoConsultation;
            }
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


    public class PractitionerId
    {
    
        public PractitionerId(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }
        public string Id { get; set; }
    }
}
