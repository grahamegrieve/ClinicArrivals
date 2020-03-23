using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.Appointment;

namespace ClinicArrivals.Models
{
    /// <summary>
    ///   This is the core logic of the ClinicalArrivals application; it's where all the processing logic lives
    ///   
    ///   There's 3 different ways that this gets called:
    ///     * every X seconds (typically, 30), processing today's appointments 
    ///     * every 5 minutes, processing the appointments for the next 3 days 
    ///     * every X seconds, processing incoming SMS messages
    ///     
    ///   Note: this class coordinates 3 different storages: twilio, the PMS, and it's own internal local storage 
    ///         actions are often a commit to 2 or 3 of those storages. There's no way to do a transction, so 
    ///         we assume that the likelihood of failure is twilio > PMS > local storage - so we do changes in that order
    /// </summary>
    public class MessagingEngine
    {
        private List<String> whitelist = new List<string>();

        // this specifies what day it is today. In production, this is *always* the current date/time 
        // but it can be overridden to another date/time by testing code (that makes it easier to manage the tests).
        public DateTime TimeNow { get; set; }

        /// <summary>
        ///  Used to send a message to the patient
        /// </summary>
        public ISmsProcessor SmsSender { get; set; }

        /// <summary>
        ///  Used to update the stored state of a single appointment, after a message has been sent, or to store SMS messages that cannot be understood 
        /// </summary>
        public IArrivalsLocalStorage Storage { get; set; }

        /// <summary>
        /// Provides the services to update the state of the Appointment on the PMS
        /// </summary>
        public IFhirAppointmentUpdater AppointmentUpdater { get; set; }

        /// <summary>
        /// Provides template processing services to turn a template + variables into ready to go text
        /// </summary>
        public TemplateProcessor TemplateProcessor { get; set; }

        /// <summary>
        /// Connects to the VideoConferencing engine
        /// </summary>
        public IVideoConferenceManager VideoManager { get; set; }

        /// <summary>
        ///   Logging errors somewhere..
        /// </summary>
        public ILoggingService Logger { get; set; }

        public ObservableCollection<DoctorRoomLabelMapping> RoomMappings { get; set; }
        public ObservableCollection<SmsMessage> UnprocessableMessages { get; set; } 

        // Call this before using the 
        public void Initialise(Settings settings)
        {
            TimeNow = DateTime.Now;
            foreach (string s in settings.PhoneWhiteList.Split(','))
            {
                whitelist.Add(s.Trim());
            }
        }

        /// <summary>
        /// This method is called every X seconds to process any changes to the appointments on the PMS side 
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">The current information from the PMS</param>
        public void ProcessTodaysAppointments(List<PmsAppointment> appointments)
        {
            // pseudo code
            // for each incoming appointment
            //   is it new?- add it to the stored list
            //   has the status changed from arrived to fulfilled? - send the invite message if it's not a TeleHealth consultation
            //   if the appointment is within 3 hours, and the screening message hasn't been sent, send it 
            //   if the appointment is within 10 minutes a TeleHealth consultation, and the setup message hasn't been sent, send it 
            foreach (var appt in appointments.Where(n => IsUseablePhoneNumber(n.PatientMobilePhone) && IsToday(n.AppointmentStartTime)))
            {
                try
                {
                    if (appt.ExternalData.ArrivalStatus == AppointmentStatus.Arrived && appt.ArrivalStatus == AppointmentStatus.Fulfilled)
                    {
                        Dictionary<string, string> vars = new Dictionary<string, string>();
                        vars.Add("room", FindRoomNote(appt.PractitionerFhirID));
                        SmsMessage msg = new SmsMessage(NormalisePhoneNumber(appt.PatientMobilePhone), TemplateProcessor.processTemplate(MessageTemplate.MSG_APPT_READY, appt, vars));
                        SmsSender.SendMessage(msg);
                        LogMsg(OUT, msg, "invite patient to come in", appt);
                        appt.ExternalData.ArrivalStatus = appt.ArrivalStatus;
                        Storage.SaveAppointmentStatus(appt);
                    }
                    else if (appt.ArrivalStatus == AppointmentStatus.Booked && IsInTimeWindow(appt.AppointmentStartTime, 180) && !appt.ExternalData.ScreeningMessageSent)
                    {
                        SmsMessage msg = new SmsMessage(NormalisePhoneNumber(appt.PatientMobilePhone), TemplateProcessor.processTemplate(MessageTemplate.MSG_SCREENING, appt, null));
                        SmsSender.SendMessage(msg);
                        LogMsg(OUT, msg, "send out screening message", appt);
                        appt.ExternalData.ScreeningMessageSent = true;
                        Storage.SaveAppointmentStatus(appt);
                    }
                    else if (appt.ArrivalStatus == AppointmentStatus.Booked && appt.IsVideoConsultation && IsInTimeWindow(appt.AppointmentStartTime, 10) && !appt.ExternalData.VideoInviteSent)
                    {
                        Dictionary<string, string> vars = new Dictionary<string, string>();
                        vars.Add("url", VideoManager.getConferenceUrl(appt.AppointmentFhirID));
                        SmsMessage msg = new SmsMessage(NormalisePhoneNumber(appt.PatientMobilePhone), TemplateProcessor.processTemplate(MessageTemplate.MSG_VIDEO_INVITE, appt, vars));
                        SmsSender.SendMessage(msg);
                        LogMsg(OUT, msg, "invite to video", appt);
                        appt.ExternalData.VideoInviteSent = true;
                        Storage.SaveAppointmentStatus(appt);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(ERR, "Exception processing " + appt.AppointmentFhirID + ": " + e.Message);
                }
            }
        }

        private const bool IN = true;
        private const bool OUT = false;
        private const int MSG = 1;
        private const int ERR = 2;

        private void LogMsg(bool isIn, SmsMessage msg, string op, PmsAppointment appt)
        {
            Logger.Log(MSG, (isIn ? "Receive from " : "Send to ") + msg.phone + ": " + msg.message + (op != null ? " (" + op + " on " + (appt == null ? "null" : appt.AppointmentFhirID) + ")" : ""));
        }

        /// <summary>
        /// This method is called every X minutes to process any changes to the future appointments on the PMS side.
        /// Typically, this covers the next 2 days in the future (not including today, since we don't send the registration message if the appointment is made today)
        /// 
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">The current information from the PMS</param>
        public void ProcessUpcomingAppointments(List<PmsAppointment> appointments)
        {
            // pseudo code
            // for each incoming appointment
            //   is it new - send the pre-registration message, and add it to stored
            foreach (var appt in appointments.Where(n => IsUseablePhoneNumber(n.PatientMobilePhone) && IsNearFuture(n.AppointmentStartTime))) // we only send these messages 2-3 days in the future
            {
                try
                {
                    if (!appt.ExternalData.PostRegistrationMessageSent)
                    {
                        SmsMessage msg = new SmsMessage(NormalisePhoneNumber(appt.PatientMobilePhone), TemplateProcessor.processTemplate(MessageTemplate.MSG_REGISTRATION, appt, null));
                        SmsSender.SendMessage(msg);
                        LogMsg(OUT, msg, "send registration message", appt);
                        appt.ExternalData.PostRegistrationMessageSent = true;
                        Storage.SaveAppointmentStatus(appt);
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(ERR, "Exception processing " + appt.AppointmentFhirID + ": " + e.Message);
                }
            }
        }

        /// <summary>
        /// This method is called every X seconds to process any incoming SMS appointments
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">Sms Messages received since last poll</param>
        public void ProcessIncomingMessages(List<PmsAppointment> appts, IEnumerable<SmsMessage> incoming)
        {
            foreach (var msg in incoming)
            {
                try
                {
                    LogMsg(IN, msg, null, null);
                    // pseudo code 
                    // find the candidate appointments for this mobile phone 
                    // if there aren't any - return the 'please call reception message', and drop this message
                    // if there's more than one, pick one
                    // ok, now we have appointment and message
                    // if we sent an invitation for a video conference 
                    //   process as a response to the invitation
                    // else if we are expecting a response to the screening
                    //   process as a response to the screening
                    // else if we are expecting them to arrive 
                    //   process as an arrival messages
                    // else
                    //   we are not expecting a response - send message explaining that 
                    List<PmsAppointment> candidates = FindCandidateAppointments(appts, msg.phone);
                    if (candidates.Count == 0)
                    {
                        HandleUnknownMessage(msg);
                    }
                    else
                    {
                        PmsAppointment appt = candidates.Count == 1 ? candidates[0] : ChooseRelevantAppointment(candidates, msg);
                        if (appt == null || !IsUseablePhoneNumber(appt.PatientMobilePhone))
                        {
                            ProcessUnexpectedResponse(candidates[0], msg);
                        }
                        else if (appt.ExternalData.VideoInviteSent)
                        {
                            ProcessVideoInviteResponse(appt, msg);
                        }
                        else if (appt.ExternalData.ScreeningMessageSent && !appt.ExternalData.ScreeningMessageResponse)
                        {
                            ProcessScreeningResponse(appt, msg);
                        }
                        else if (appt.ArrivalStatus == AppointmentStatus.Booked)
                        {
                            ProcessArrivalMessage(appt, msg);
                        }
                        else
                        {
                            ProcessUnexpectedResponse(appt, msg);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log(ERR, "Exception processing message: " + e.Message);
                }
            }
        }

        private bool IsUseablePhoneNumber(string num)
        {
            num = NormalisePhoneNumber(num);
            return whitelist.Count == 0 ? true : whitelist.Contains(num);
        }

        private void HandleUnknownMessage(SmsMessage msg)
        {
            // a future possible enhancement is to ask the user which patient the appointment is for; this will smooth the work flow, but the response 
            // processing might be complicated. can it be just a Medicare number and date? 
            SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_UNKNOWN_PH, null, null));
            SmsSender.SendMessage(rmsg);
            LogMsg(OUT, rmsg, "handle unknown message", null);
            UnprocessableMessages.Add(msg);
        }

        private void ProcessVideoInviteResponse(PmsAppointment appt, SmsMessage msg)
        {
            if (MessageMatches(msg.message, "joined", "ok", "j"))
            {
                // twilio:
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_VIDEO_THX, appt, null));
                SmsSender.SendMessage(rmsg);
                LogMsg(OUT, rmsg, "accept video response", appt);
                appt.ArrivalStatus = AppointmentStatus.Arrived;

                // PMS:
                AppointmentUpdater.SaveAppointmentStatusValue(appt);
                // local storage:
                appt.ExternalData.ArrivalStatus = appt.ArrivalStatus;
                Storage.SaveAppointmentStatus(appt);
            }
            else
            {
                // we haven't understood it 
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_VIDEO, appt, null));
                SmsSender.SendMessage(rmsg);
                LogMsg(OUT, rmsg, "fail to process video response", appt);
                UnprocessableMessages.Add(msg);
            }
        }
        private void ProcessScreeningResponse(PmsAppointment appt, SmsMessage msg)
        {
            // the patient should respond with "yes" or "no" but they may not bother and just respond with "arrived"
            // of course they might respond with anything else that we can't understand, so we'll explain apologetically if they do
            if (MessageMatches(msg.message, "yes", "y"))
            {
                // twilio: 
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_SCREENING_YES, appt, null));
                SmsSender.SendMessage(rmsg);
                LogMsg(OUT, rmsg, "process screening response 'yes'", appt);

                // PMS:
                appt.IsVideoConsultation = true;
                AppointmentUpdater.SaveAppointmentAsVideoMeeting(appt, "Video URL: " + VideoManager.getConferenceUrl(appt.AppointmentFhirID));

                // local storage
                appt.ExternalData.ScreeningMessageResponse = true;
                Storage.SaveAppointmentStatus(appt);
            }
            else if (MessageMatches(msg.message, "no", "n"))
            {
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_SCREENING_NO, appt, null));
                SmsSender.SendMessage(rmsg);
                LogMsg(OUT, rmsg, "process screening response 'no'", appt);
                appt.ExternalData.ScreeningMessageResponse = true;
                appt.IsVideoConsultation = false;
                Storage.SaveAppointmentStatus(appt);
            }
            else if (MessageMatches(msg.message, "arrived", "here", "a"))
            {
                ProcessArrivalMessage(appt, msg);
            }
            else
            {
                // we haven't understood it 
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_SCREENING, appt, null));
                SmsSender.SendMessage(rmsg);
                LogMsg(OUT, rmsg, "fail to process screening response", appt);
                UnprocessableMessages.Add(msg);
            }
        }

        private void ProcessArrivalMessage(PmsAppointment appt, SmsMessage msg)
        {
            if (MessageMatches(msg.message, "arrived", "here", "a"))
            {
                // twilio:
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_ARRIVED_THX, appt, null));
                SmsSender.SendMessage(rmsg);
                LogMsg(OUT, rmsg, "process arrival message", appt);
                appt.ArrivalStatus = AppointmentStatus.Arrived;
                // PMS:
                AppointmentUpdater.SaveAppointmentStatusValue(appt);
                // local storage
                appt.ExternalData.ScreeningMessageResponse = true;
                appt.ExternalData.ArrivalStatus = AppointmentStatus.Arrived;
                Storage.SaveAppointmentStatus(appt);
            }
            else
            {
                // we haven't understood it 
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_ARRIVING, appt, null));
                SmsSender.SendMessage(rmsg);
                LogMsg(OUT, rmsg, "fail to process arrival message", appt);
                UnprocessableMessages.Add(msg);
            }
        }

        private void ProcessUnexpectedResponse(PmsAppointment appt, SmsMessage msg)
        {
            SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_UNEXPECTED, appt, null));
            SmsSender.SendMessage(rmsg);
            LogMsg(OUT, rmsg, "unexpected message", appt);
            UnprocessableMessages.Add(msg);
        }

        private PmsAppointment ChooseRelevantAppointment(List<PmsAppointment> candidates, SmsMessage msg)
        {
            if (candidates.Count > 2)
            {
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_TOO_MANY_APPOINTMENTS, candidates[0], null));
                SmsSender.SendMessage(rmsg);
                LogMsg(OUT, rmsg, "Too many candidates", candidates[0]);
                UnprocessableMessages.Add(msg);
            }
            else
            {
                // pseudo code: 
                // if the two appointments are at the same time, we only care about the first one
                // otherwise, in principle we are interested in the first one, unless the message response belongs to an earlier cycle ("yes" / "no") and we're waiting for that
                var appt1 = candidates[0].AppointmentStartTime < candidates[1].AppointmentStartTime ? candidates[0] : candidates[1];
                var appt2 = candidates[0].AppointmentStartTime < candidates[1].AppointmentStartTime ? candidates[1] : candidates[0];
                if (MessageMatches(msg.message, "yes", "no"))
                {
                    foreach (var appt in candidates)
                    {
                        if (appt.ExternalData.ScreeningMessageSent && !appt.ExternalData.ScreeningMessageResponse)
                        {
                            return appt;
                        }
                    }
                    return null;
                }
                else if (MessageMatches(msg.message, "joined"))
                {
                    foreach (var appt in candidates)
                    {
                        if (appt.ExternalData.VideoInviteSent)
                        {
                            return appt;
                        }
                    }
                    return null;
                }
                else if (MessageMatches(msg.message, "arrived"))
                {
                    foreach (var appt in candidates)
                    {
                        if (appt.ArrivalStatus == AppointmentStatus.Booked)
                        {
                            return appt;
                        }
                    }
                    return null;
                }
                else
                {
                    SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_TOO_MANY_APPOINTMENTS, candidates[0], null));
                    SmsSender.SendMessage(rmsg);
                    LogMsg(OUT, rmsg, "can't choose appointment", appt1);
                    UnprocessableMessages.Add(msg);
                }
            }
            return null;
        }

        private List<PmsAppointment> FindCandidateAppointments(List<PmsAppointment> appts, string phone)
        {
            List<PmsAppointment> list = new List<PmsAppointment>();
            foreach (var appt in appts)
            {
                if (IsSamePhone(appt.PatientMobilePhone, phone))
                {
                    list.Add(appt);
                }
            }
            return list;
        }

        private bool MessageMatches(string msg, params string[] values)
        {
            foreach (string s in values)
            {
                if (MessageTextMatches(msg, s))
                {
                    return true;
                }
            }
            return false;
        }

        private bool MessageTextMatches(string msg, string value)
        {
            msg = msg.ToLower();
            StringBuilder m = new StringBuilder();
            foreach (char ch in msg)
            {
                if (Char.IsLetterOrDigit(ch))
                {
                    m.Append(ch);
                }
            }
            return m.ToString() == value;
        }

        private bool IsSamePhone(string p1, string p2)
        {
            p1 = NormalisePhoneNumber(p1);
            p2 = NormalisePhoneNumber(p2);
            return p1 == p2;
        }

        private string NormalisePhoneNumber(string p1)
        {
            p1 = p1?.Replace(" ", "");
            if (p1?.StartsWith("04") == true)
            {
                p1 = "+614" + p1.Substring(2);
            }
            return p1;
        }

        private Boolean IsInTimeWindow(DateTime start, int minutes)
        {
            DateTime endWindow = TimeNow.AddMinutes(minutes);
            return TimeNow <= start && endWindow > start;
        }

        private Boolean IsToday(DateTime start)
        {
            return TimeNow.Date == start.Date;
        }

        private Boolean IsNearFuture(DateTime start)
        {
            return TimeNow.Date < start.Date;
        }

        private String FindRoomNote(string practId)
        {
            foreach (var rm in RoomMappings)
            {
                if (rm.PractitionerFhirID == practId)
                {
                    return rm.LocationName;
                }
            }
            return "";
        }

    }
}
