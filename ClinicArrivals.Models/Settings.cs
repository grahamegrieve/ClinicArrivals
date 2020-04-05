using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using IWshRuntimeLibrary;

namespace ClinicArrivals.Models
{
    public enum VideoConferencingType
    {
        OpenVidu,
        Jitsi
        
    }

    [AddINotifyPropertyChangedInterface]
    public class Settings
    {
        [JsonIgnore]
        public ICommand Save { get; set; }
        [JsonIgnore]
        public ICommand Reload { get; set; }
        [JsonIgnore]
        public ICommand TestSms { get; set; }

        public string FromTwilioMobileNumber { get; set; }
        public string ACCOUNT_SID { get; set; }
        public string AUTH_TOKEN { get; set; }
        public int PollIntervalSeconds { get; set; }
        public int RegistrationPollIntervalSeconds { get; set; }
        public bool ExamplesServer { get; set; }
        public Guid SystemIdentifier { get; private set; }

        public string AdministratorPhone { get; set; }

        public string OpenViduSecret { get; set; }

        public void AllocateNewSystemIdentifier() { SystemIdentifier = Guid.NewGuid(); }

        public string DeveloperPhoneNumber { get; set; }

        public int MessageLimitForNumber { get; set; }

        public string PhoneWhiteList { get; set; }

        public bool IsDoingVideo { get; set; }
        public VideoConferencingType VideoType { get; set; }

        public string PMSProfileName { get; set; }
        public string PMSLicenseKey { get; set; }

        public int MinutesBeforeScreeningMessage { get; set; }
        public int MinutesBeforeVideoInvitation { get; set; }

        public bool AutoStartServices { get; set; }

        private bool _loadOnStartup;
        private readonly string SHORTCUT_TO_APP =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Microsoft\\Windows\\Start Menu\\Programs\\ClinicArrivals Authors\\ClinicArrivals.lnk";

        public bool LoadOnStartup
        {
            get { return _loadOnStartup; }
            set 
            {
                if (_loadOnStartup == value)
                {
                    return;
                }

                _loadOnStartup = value;
                UpdateStartupShortcut(value);
            }
        }

        private void UpdateStartupShortcut(bool loadOnStartup)
        {
            if (loadOnStartup)
            {
                createStartupShortcut();
            } else
            {
                removeStartupShortcut();
            }
        }

        // credit: https://gist.github.com/Mierenga/a8a1fcdbee5555ac9249aab7837da0bd
        private string getShortcutPath()
        {
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return System.IO.Path.Combine(startupPath, "ClinicArrivals.lnk");
        }

        private void createStartupShortcut()
        {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(getShortcutPath());
            shortcut.Description = "Open ClinicArrivals on computer start";
            shortcut.TargetPath = SHORTCUT_TO_APP;
            shortcut.Save();
        }

        private void removeStartupShortcut()
        {
            string shortcutPath = getShortcutPath();
            if (System.IO.File.Exists(shortcutPath))
            {
                System.IO.File.Delete(shortcutPath);
            }
        }

        [JsonIgnore]
        public bool UpdateAvailable { get; set; }
        // the latest update that the admin received an sms text about
        public string AdminNotifiedOfUpdate { get; set; }

        public void CopyFrom(Settings other)
        {
            FromTwilioMobileNumber = other.FromTwilioMobileNumber;
            ACCOUNT_SID = other.ACCOUNT_SID;
            AUTH_TOKEN = other.AUTH_TOKEN;
            PollIntervalSeconds = other.PollIntervalSeconds;
            RegistrationPollIntervalSeconds = other.RegistrationPollIntervalSeconds;
            ExamplesServer = other.ExamplesServer;
            SystemIdentifier = other.SystemIdentifier;
            DeveloperPhoneNumber = other.DeveloperPhoneNumber;
            PhoneWhiteList = other.PhoneWhiteList;
            VideoType = other.VideoType;
            PMSProfileName = other.PMSProfileName;
            PMSLicenseKey = other.PMSLicenseKey;
            IsDoingVideo = other.IsDoingVideo;
            OpenViduSecret = other.OpenViduSecret;
            AdministratorPhone = other.AdministratorPhone;
            MinutesBeforeScreeningMessage = other.MinutesBeforeScreeningMessage;
            MinutesBeforeVideoInvitation = other.MinutesBeforeVideoInvitation;
            AutoStartServices = other.AutoStartServices;
            MessageLimitForNumber = other.MessageLimitForNumber;
    }
}
}
