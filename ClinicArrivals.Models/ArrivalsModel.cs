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

        public string StatusBarMessage { get; set; }
    }
}
