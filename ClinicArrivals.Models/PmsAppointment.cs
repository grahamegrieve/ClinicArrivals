using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using static Hl7.Fhir.Model.Appointment;

namespace ClinicArrivals.Models
{
    [AddINotifyPropertyChangedInterface]
    public class PmsAppointment
    {
        // Things from the PMS 
        public string PatientFhirID { get; set; }
        public string PatientName { get; set; }
        public string PatientMobilePhone { get; set; }
        public string PractitionerName { get; set; }
        public string PractitionerFhirID { get; set; }
        public string AppointmentFhirID { get; set; }
        public AppointmentStatus ArrivalStatus { get; set; }
        public DateTime AppointmentStartTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public Boolean IsVideoConsultation { get; set; }

        /// <summary>
        /// This is the transient extended content that is not stored in the PMS FHIR Server
        /// that is used for tracking processing only
        /// </summary>
        public PmsAppointmentExtendedData ExternalData { get; set; } = new PmsAppointmentExtendedData();
    }

    /// <summary>
    /// This is the transient extended content that is not stored in the FHIR Server
    /// that is used for tracking processing only
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class PmsAppointmentExtendedData
    {
        /// <summary>
        /// The status of the appointment when it was last processed
        /// </summary>
        public AppointmentStatus ArrivalStatus { get; set; }

        // Record of actions we've taken with the Appointment
        public bool PostRegistrationMessageSent { get; set; }
        public bool ScreeningMessageSent { get; set; }
        public bool ScreeningMessageResponse { get; set; }
        public bool VideoInviteSent { get; set; }

        /// <summary>
        /// This is last message that came in via SMS (will be in memory only - cleared if service restarts)
        /// </summary>
        public string LastPatientMessage { get; set; }
    }
}
