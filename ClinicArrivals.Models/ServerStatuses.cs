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
        public ServerStatus Oridashi { get; set; } = new ServerStatus() { Name = "Oridashi FHIR Server", Status = ServerStatusEnum.Stopped };
        public ServerStatus IncomingSmsReader { get; set; } = new ServerStatus() { Name = "Incoming SMS Reader", Status = ServerStatusEnum.Stopped };
        public ServerStatus AppointmentScanner { get; set; } = new ServerStatus() { Name = "Appointment Scanner", Status = ServerStatusEnum.Stopped };
        public ServerStatus UpcomingAppointmentProcessor { get; set; } = new ServerStatus() { Name = "Registration Sender", Status = ServerStatusEnum.Stopped };
    }

    public enum ServerStatusEnum
    {
        Starting, Stopping, Running, Processing, Stopped, Error
    }

    [AddINotifyPropertyChangedInterface]
    public class ServerStatus
    {
        public string Name { get; set; }
        public string Error { get; set; }

        public ServerStatusEnum Status { get; set; }
        public int Count { get; set; }

        private bool _hasLastUse = false;
        private DateTime _lastUse;
        public DateTime LastUse { get { return _lastUse; } set { _hasLastUse = true; _lastUse = value; } }

        /// <summary>
        /// Starting, Stopping, Running, Stopped
        /// </summary>
        public string CurrentStatus { get { return GenStatus();  } }

        private string GenStatus()
        {
            if (Status == ServerStatusEnum.Running || Status == ServerStatusEnum.Processing)
            {
                if (!_hasLastUse && Count == 0) 
                {
                    return Status.ToString()+". {not doing anything)";
                } 
                else if (Count == 0)
                {
                    return Status.ToString() + ". {Last Used = " + (DateTime.Now - LastUse).TotalSeconds + ")";
                } else
                {
                    return Status.ToString() + ". {" + Count+" transactions. Last Used = " + (DateTime.Now - LastUse).TotalSeconds + ")";
                }
            }
            else if (Error != null)
            {
                return Status.ToString()+" (Error = "+Error+")";
            }
            else
            {
                return Status.ToString();
            }
        }

        // Command Handlers (so the UI can interact - disable buttons etc)
        public ICommand Start { get; set; }
        public ICommand Stop { get; set; }

        public void Use(int Items)
        {
            Count += Items;
            LastUse = DateTime.Now;
        }
    }
}
