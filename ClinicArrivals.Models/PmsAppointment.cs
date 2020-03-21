using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace ClinicArrivals.Models
{
    [AddINotifyPropertyChangedInterface]
    public class PmsAppointment
    {
        public string PatientFhirID { get; set; }
        public string PatientName { get; set; }

        public byte[] PatientPhoto { get; set; }

        /// <summary>
        /// This is last message that came in via SMS (will be in memory only - cleared if service restarts)
        /// </summary>
        public string LastPatientMessage { get; set; }

        public string PatientMobilePhone { get; set; }

        public string PractitionerName { get; set; }
        public string PractitionerFhirID { get; set; }

        /// <summary>
        /// e.g. Allocated room number
        /// If we can do a mapping on this - probably going to be local file mapping? (prac to Room)
        /// </summary>
        public string LocationName { get; set; }

        public string AppointmentFhirID { get; set; }
        public string ArrivalStatus { get; set; } 
        public string AppointmentStartTime { get; set; }
        public string ArrivalTime { get; set; }

        public bool ReadyToBeNotifiedToComeInside { get; set; } 

        public bool NotifiedToComeInside { get; set; }
    }
}
