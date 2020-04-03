﻿using Newtonsoft.Json;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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

        public string PhoneWhiteList { get; set; }

        public bool IsDoingVideo { get; set; }
        public VideoConferencingType VideoType { get; set; }

        public string PMSProfileName { get; set; }
        public string PMSLicenseKey { get; set; }

        public int MinutesBeforeScreeningMessage { get; set; }
        public int MinutesBeforeVideoInvitation { get; set; }

        public bool AutoStartServices { get; set; }

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
    }
}
}
