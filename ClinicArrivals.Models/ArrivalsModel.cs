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
            DisplayingDate = DateTime.Now.Date;
        }

        /// <summary>
        /// The Status of each of the server processes running in the App
        /// </summary>
        public ServerStatuses serverStatuses { get; set; } = new ServerStatuses();

        public DateTime DisplayingDate { get; set; }
        public ICommand SaveRoomMappings { get; set; }
        public ICommand ReloadRoomMappings { get; set; }
        public ICommand SaveTemplates { get; set; }
        public ICommand ReloadTemplates { get; set; }
        public ICommand SeeTemplateDocumentation { get; set; }
        public ICommand ClearUnproccessedMessages { get; set; }

        public Settings Settings { get; set; } = new Settings();

        public IArrivalsLocalStorage Storage { get; set; }

        public string StatusBarMessage { get; set; }

        public ObservableCollection<PmsAppointment> Waiting { get; private set; } = new ObservableCollection<PmsAppointment>();

        public ObservableCollection<PmsAppointment> Expecting { get; private set; } = new ObservableCollection<PmsAppointment>();

        public ObservableCollection<PmsAppointment> Appointments { get; private set; } = new ObservableCollection<PmsAppointment>();

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
        public ObservableCollection<DoctorRoomLabelMapping> RoomMappings { get; private set; } = new ObservableCollection<DoctorRoomLabelMapping>();
    }
}
