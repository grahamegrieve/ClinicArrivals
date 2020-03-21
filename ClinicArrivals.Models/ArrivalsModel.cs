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
        public ArrivalsModel()
        {

        }

        public Settings Settings { get; set; } = new Settings();

        public IArrivalsLocalStorage Storage { get; set; }

        public ObservableCollection<PmsAppointment> Waiting { get; set; } = new ObservableCollection<PmsAppointment>();

        public ObservableCollection<PmsAppointment> Expecting { get; set; } = new ObservableCollection<PmsAppointment>();

        public string StatusBarMessage { get; set; }

        /// <summary>
        /// Incoming SMS messages that couldn't be processed
        /// </summary>
        public ObservableCollection<SmsMessage> UnprocessableMessages { get; set; } = new ObservableCollection<SmsMessage>();

        /// <summary>
        /// List of templates for each of the different message types
        /// </summary>
        public ObservableCollection<KeyValuePair<string, string>> Templates { get; set; } = new ObservableCollection<KeyValuePair<string, string>>();

        /// <summary>
        /// Room Labels/Doctor mappings
        /// </summary>
        public ObservableCollection<DoctorRoomLabelMappings> RoomMappings { get; set; } = new ObservableCollection<DoctorRoomLabelMappings>();
    }
}
