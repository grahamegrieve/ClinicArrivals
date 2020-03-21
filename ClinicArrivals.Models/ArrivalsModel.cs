using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace ClinicArrivals.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ArrivalsModel
    {
        public ObservableCollection<PmsAppointment> Waiting { get; set; } = new ObservableCollection<PmsAppointment>();

        public ObservableCollection<PmsAppointment> Expecting { get; set; } = new ObservableCollection<PmsAppointment>();

        public ObservableCollection<PmsAppointment> Fulfilled { get; set; } = new ObservableCollection<PmsAppointment>();

        public string StatusBarMessage { get; set; }

        // Incoming SMS messages that couldn't be processed
        public ObservableCollection<SmsMessage> UnprocessableMessages { get; set; } = new ObservableCollection<SmsMessage>();

        // List of templates for each of the differrent message types
        public ObservableCollection<KeyValuePair<string, string>> Templates { get; set; } = new ObservableCollection<KeyValuePair<string, string>>();

        // Room Labels/Doctor mappings
        public ObservableCollection<DoctorRoomLabelMappings> RoomMappings { get; set; } = new ObservableCollection<DoctorRoomLabelMappings>();
    }
}
