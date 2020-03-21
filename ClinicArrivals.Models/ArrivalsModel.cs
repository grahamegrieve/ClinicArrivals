using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace ClinicArrivals.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ArrivalsModel
    {
        public ArrivalsModel()
        {
            DisplayingDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
        }

        public string DisplayingDate { get; set; }
        public ICommand SaveRoomMappings { get; set; }
        public ICommand ReloadRoomMappings { get; set; }
        public ICommand SaveTemplates { get; set; }
        public ICommand ReloadTemplates { get; set; }

        public Settings Settings { get; set; } = new Settings();

        public IArrivalsLocalStorage Storage { get; set; }

        public ObservableCollection<PmsAppointment> Waiting { get; set; } = new ObservableCollection<PmsAppointment>();

        public ObservableCollection<PmsAppointment> Expecting { get; set; } = new ObservableCollection<PmsAppointment>();

        public ObservableCollection<PmsAppointment> Fulfilled { get; set; } = new ObservableCollection<PmsAppointment>();

        public string StatusBarMessage { get; set; }

        /// <summary>
        /// Incoming SMS messages that couldn't be processed
        /// </summary>
        public ObservableCollection<SmsMessage> UnprocessableMessages { get; private set; } = new ObservableCollection<SmsMessage>();

        /// <summary>
        /// List of templates for each of the different message types
        /// </summary>
        public ObservableCollection<MessageTemplate> Templates { get; private set; } = new ObservableCollection<MessageTemplate>();

        /// <summary>
        /// Room Labels/Doctor mappings
        /// </summary>
        public ObservableCollection<DoctorRoomLabelMappings> RoomMappings { get; private set; } = new ObservableCollection<DoctorRoomLabelMappings>();
    }
}
