using Newtonsoft.Json;
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
    public class Settings
    {
        [JsonIgnore]
        public ICommand Save { get; set; }
        [JsonIgnore]
        public ICommand Reload { get; set; }


        public string FromTwilioMobileNumber { get; set; }
        public string ACCOUNT_SID { get; set; }
        public string AUTH_TOKEN { get; set; }
        public int PollIntervalSeconds { get; set; }
        public string IntroSmsMessage { get; set; }
        public bool ExamplesServer { get; set; }

        public string DeveloperPhoneNumber { get; set; }

        public void CopyFrom(Settings other)
        {
            FromTwilioMobileNumber = other.FromTwilioMobileNumber;
            ACCOUNT_SID = other.ACCOUNT_SID;
            AUTH_TOKEN = other.AUTH_TOKEN;
            PollIntervalSeconds = other.PollIntervalSeconds;
            IntroSmsMessage = other.IntroSmsMessage;
            ExamplesServer = other.ExamplesServer;
            DeveloperPhoneNumber = other.DeveloperPhoneNumber;
        }
    }
}
