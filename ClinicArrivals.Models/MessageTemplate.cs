using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Models
{
    public class MessageTemplate
    {
        public const string MSG_REGISTRATION = "Registration";
        public const string MSG_CANCELLATION = "Cancellation";
        public const string MSG_SCREENING = "ConsiderTeleHealth";
        public const string MSG_PREAPP = "PreApppointment";
        public const string MSG_UNKNOWN_PAT = "UnknownPatient";
        public const string MSG_APPT_READY = "DoctorReady";
        public const string MSG_VIDEO_INVITE = "VideoInvite";

        public MessageTemplate(string type, string template = null)
        {
            MessageType = type;
            Template = template;
        }

        /// <summary>
        /// The Type of message template
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// The actual template of the message (Razor or simple string replace)
        /// </summary>
        public string Template { get; set; }
    }
}
