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
        public const string MSG_UNKNOWN_PH = "UnknownPhone";
        public const string MSG_TOO_MANY_APPOINTMENTS = "TooManyAppointments";
        public const string MSG_UNEXPECTED = "Unexpected";
        public const string MSG_SCREENING = "ConsiderTeleHealth";
        public const string MSG_SCREENING_NOVIDEO = "CantConsiderTeleHealth";
        public const string MSG_SCREENING_YES = "ScreeningYesToVideo";
        public const string MSG_SCREENING_NO = "ScreeningNoToVideo";
        public const string MSG_DONT_UNDERSTAND_SCREENING = "ScreeningDontUnderstand";
        public const string MSG_VIDEO_INVITE = "VideoInvite";
        public const string MSG_VIDEO_THX = "VideoThanks";
        public const string MSG_DONT_UNDERSTAND_VIDEO = "VideoDontUnderstand";
        public const string MSG_ARRIVED_THX = "ArrivedThanks";
        public const string MSG_DONT_UNDERSTAND_ARRIVING = "ArrivingDontUnderstand";
        public const string MSG_APPT_READY = "DoctorReady";
        public const string MSG_VIDEO_WELCOME = "Video";

        public const string DEF_MSG_REGISTRATION = "Patient {{Patient.name}} has an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}. 3 hours prior to the appointment, you will be sent an SMS referring to a COVID-19 screening check to decide whether you should talk to the doctor by phone/video rather than seeing the doctor in person. Please do not respond to this message";
        public const string DEF_MSG_CANCELLATION = "The appointment for Patient {{Patient.name}} with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}} has been cancelled";
        public const string DEF_MSG_UNKNOWN_PH = "The phone number you sent the message from is not associated with an appointment to see the doctor today. Please phone [clinic number] for help";
        public const string DEF_MSG_TOO_MANY_APPOINTMENTS = "The robot processing this message couldn't figure out which appointment of multiple for this day that this message was about. Please phone [clinic number] for help";
        public const string DEF_MSG_UNEXPECTED = "Patient {{Patient.name}} has an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}, but this robot is not expecting a message right now";
        public const string DEF_MSG_SCREENING = "Patient {{Patient.name}} does have an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}. If you have symptoms of Covid-19, or exposure to a known case, you MUST choose to talk to the doctor by telephone/video, otherwise, you should choose to this unless you really need to come to the clinic. Respond to this message with YES to choose to telephone/video consultation, otherwise respond with NO";
        public const string DEF_MSG_SCREENING_NOVIDEO = "Patient {{Patient.name}} has an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}. When you arrive at the clinic, stay in your car (or outside the clinic) and reply \"arrived\" to this message. If you have any potential symptoms of Covid-19, or exposure to a known case, you MUST advise the Doctor and staff by telephone in advance of your appointment.";
        public const string DEF_MSG_SCREENING_YES = "Thank you. Do not come to the doctor's clinic. Your doctor will call you for your appointment. When you are ready for your appointment, reply to this message with the word 'waiting'. If the doctor wants to see you by video, they will ask you to follow a link you will be sent before the appointment. You can join from any computer or smartphone. For instructions, see https://bit.ly/2vFGl2c";
        public const string DEF_MSG_SCREENING_NO = "Thank you. When you arrive at the clinic, stay in your car (or outside the clinic) and reply \"arrived\" to this message";
        public const string DEF_MSG_DONT_UNDERSTAND_SCREENING = "The robot processing this message didn't understand your response. Please answer yes or no, or phone [clinic number] for help";
        public const string DEF_MSG_VIDEO_INVITE = "Patient {{Patient.name}} does have an appointment with {{Practitioner.name}} at {{Appointment.start.time}} is happening soon. The doctor will ring on this number. If the doctor wants to see you, the link is {{url}}. Reply \"ready\" when you are ready";
        public const string DEF_MSG_VIDEO_THX = "Thank you. The Doctor will call you as soon as possible";
        public const string DEF_MSG_DONT_UNDERSTAND_VIDEO = "The robot processing this message didn't understand your response. Please just say \"ready\" when you are ready for the call";
        public const string DEF_MSG_ARRIVED_THX = "Thanks for letting us know that you're here. We'll let you know as soon as the doctor is ready for you";
        public const string DEF_MSG_DONT_UNDERSTAND_ARRIVING = "The robot processing this message and didn't understand your response. Please just say \"arrived\", or phone [clinic number] for help";
        public const string DEF_MSG_APPT_READY = "The doctor is ready to see you now. {{room}}";
        public const string DEF_MSG_VIDEO_WELCOME = "So you're going ";

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

        static public bool IsValid(string type, string msg, out string error)
        {
            error = null;
            while (msg.Contains("{{"))
            {
                int b = msg.IndexOf("{{");
                msg = msg.Substring(0, b) + "[[" + msg.Substring(b + 2);
                int e = msg.IndexOf("}}");
                if (e == -1)
                {
                    error = "No matching end }} for the {{ at index " + b + 1;
                    return false;
                } 
                else 
                {
                    msg = msg.Substring(0, e) + "]]" + msg.Substring(e + 2);
                    String n = msg.Substring(b + 2, e - b - 2);
                    if (!(isInList(n, "Patient.name", "Practitioner.name", "Appointment.start", "Appointment.start.date", "Appointment.start.time", "Patient.telecom.mobile", "Appointment.status", "Practitioner.id", "Patient.id", "Appointment.id")
                        || (type == MSG_VIDEO_INVITE && n == "url") || (type == MSG_APPT_READY && n == "room")))
                    {
                        error = "The name '"+n+"' is not valid as a token name";
                        return false;

                    }
                }
            }
            return true;
        }

        private static bool isInList(string n, params string[] vl)
        {
            foreach (var v in vl)
            {
                if (v == n)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
