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
        public const string MSG_SCREENING_YES = "ScreeningYesToVideo";
        public const string MSG_SCREENING_NO = "ScreeningNoToVideo";
        public const string MSG_DONT_UNDERSTAND_SCREENING = "ScreeningDontUnderstand";
        public const string MSG_ARRIVED_THX = "ArrivedThanks";
        public const string MSG_DONT_UNDERSTAND_ARRIVING = "ArrivingDontUnderstand";
        public const string MSG_PREAPP = "PreApppointment";
        public const string MSG_UNKNOWN_PH = "UnknownPhone";
        public const string MSG_APPT_READY = "DoctorReady";
        public const string MSG_VIDEO_INVITE = "VideoInvite";
        public const string MSG_UNEXPECTED = "Unexpected";
        public const string MSG_VIDEO_THX = "VideoThanks";
        public const string MSG_DONT_UNDERSTAND_VIDEO = "VideoDontUnderstand";
        public const string MSG_TOO_MANY_APPOINTMENTS = "TooManyAppointments";


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
