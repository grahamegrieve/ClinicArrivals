using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicArrivals.Models
{
    [AddINotifyPropertyChangedInterface]
    public class ServerStatuses
    {
        public ServerStatus Oridashi { get; set; } = new ServerStatus() { Name = "Oridashi FHIR Server", CurrentStatus="?" };
        public ServerStatus IncomingSmsReader { get; set; } = new ServerStatus() { Name = "Incoming SMS Reader", CurrentStatus = "?" };
        public ServerStatus AppointmentScanner { get; set; } = new ServerStatus() { Name = "Appointment Scanner", CurrentStatus = "?" };
    }

    [AddINotifyPropertyChangedInterface]
    public class ServerStatus
    {
        public string Name { get; set; }
        /// <summary>
        /// Starting, Stopping, Running, Stopped
        /// </summary>
        public string CurrentStatus { get; set; }

        // Command Handlers (so the UI can interact - disable buttons etc)
        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }
    }
}
