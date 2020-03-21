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

        // Record of actions we've taken with the Appointment
        public bool PostRegistrationMessageSent { get; set; }
        public bool ScreeningMessageSent { get; set; }
        public bool VideoInviteSent { get; set; }

        // Other stuff - slated for removal? (or at least, review)

        [JsonIgnore]
        public ICommand RebookAppointment { get; set; }
        [JsonIgnore]
        public ICommand ArriveAppointment { get; set; }
        [JsonIgnore]
        public ICommand StartAppointment { get; set; }


        /// <summary>
        /// This is last message that came in via SMS (will be in memory only - cleared if service restarts)
        /// </summary>
        public string LastPatientMessage { get; set; }

        /// <summary>
        /// e.g. Allocated room number
        /// If we can do a mapping on this - probably going to be local file mapping? (prac to Room)
        /// </summary>
        public string LocationName { get; set; }

        public bool ReadyToBeNotifiedToComeInside { get; set; } 

        public bool NotifiedToComeInside { get; set; }

    }
}
