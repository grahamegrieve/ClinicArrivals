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
    ///     
    /// </summary>
    public class MessagingEngine
    {
        // this specifies what day it is today. In production, this is *always* the current date/time 
        // but it can be overridden to another date/time by testing code (that makes it easier to manage the tests).
        public DateTime TimeNow { get; set; }

        /// <summary>
        ///  Used to send a message to the patient
        /// </summary>
        public ISmsProcessor SmsSender { get; set; }

        /// <summary>
        ///  Used to update the stored state of a single appointment, after a message has been sent, or to store Sms messages that cannot be understood 
        /// </summary>
        public IArrivalsLocalStorage Storage { get; set; }

        /// <summary>
        /// Provides the services to update the state of the Appointment on the PMS
        /// </summary>
        public IFhirAppointmentUpdater AppointmentUpdateer { get; set; }

        /// <summary>
        /// Provides template processing services to turn a template + variables into ready to go text
        /// </summary>
        public TemplateProcessor TemplateProcessor { get; set; }

        /// <summary>
        /// Cconnects to the VideoConferencing engine
        /// </summary>
        public IVideoConferenceManager VideoManager { get; set; }

        /// <summary>
        ///   Logging errors somewhere..
        /// </summary>
        public ILoggingService Logger { get; set; }

        public ObservableCollection<DoctorRoomLabelMapping> RoomMappings { get; set; }

        // Call this before using the 
        public void Initialise(Settings settings)
        {
            TimeNow = DateTime.Now;
        }

        /// <summary>
        /// This method is called every X seconds to process any changes to the appointments on the PMS side 
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">The current information from the PMS</param>
        public void ProcessTodaysAppointments(List<PmsAppointment> stored, List<PmsAppointment> incoming)
        {
            // pseudo code
            // for each incoming appointment
            //   is it new?- add it to the stored list
            //   has the status changed from arrived to fulfilled? - send the invite message if it's not a telehealth consultation
            //   if the appointment is within 3 hours, and the screening message hasn't been sent, send it 
            //   if the appointment is within 10 minutes a telehelth consultation, and the setup message hasn't been sent, send it 
            foreach (var appt in incoming.Where(n => n.PatientMobilePhone != null && IsToday(n.AppointmentStartTime)))
            {
                var oldAppt = findApp(stored, appt.AppointmentFhirID);
                if (oldAppt == null)
                {
                    // we don't do anything new with this; we haven't seen it before but that doesn't really make any difference. We add it to the list, and store it 
                    Storage.SaveAppointmentStatus(DateTime.Now.ToString(), appt);
                    oldAppt = appt;
                }
                if (oldAppt.ArrivalStatus == AppointmentStatus.Arrived && appt.ArrivalStatus == AppointmentStatus.Fulfilled)
                {
                    Dictionary<string, string> vars = new Dictionary<string, string>();
                    vars.Add("room", findRoomNote(appt.PractitionerFhirID));
                    SmsMessage msg = new SmsMessage(normalisePhoneNumber(appt.PatientMobilePhone), TemplateProcessor.processTemplate(MessageTemplate.MSG_APPT_READY, appt, vars));
                    SmsSender.SendMessage(msg);
                    Storage.SaveAppointmentStatus(DateTime.Now.ToString(), appt);
                }
                else if (appt.ArrivalStatus == AppointmentStatus.Booked && IsInTimeWindow(appt.AppointmentStartTime, 180) && !oldAppt.ScreeningMessageSent)
                {
                    SmsMessage msg = new SmsMessage(normalisePhoneNumber(appt.PatientMobilePhone), TemplateProcessor.processTemplate(MessageTemplate.MSG_SCREENING, appt, null));
                    SmsSender.SendMessage(msg);
                    appt.ScreeningMessageSent = true;
                    Storage.SaveAppointmentStatus(DateTime.Now.ToString(), appt);
                }
                else if (appt.ArrivalStatus == AppointmentStatus.Booked && appt.IsVideoConsultation && IsInTimeWindow(appt.AppointmentStartTime, 10) && !oldAppt.VideoInviteSent)
                {
                    Dictionary<string, string> vars = new Dictionary<string, string>();
                    vars.Add("url", VideoManager.getConferenceUrl(appt.AppointmentFhirID));
                    SmsMessage msg = new SmsMessage(normalisePhoneNumber(appt.PatientMobilePhone), TemplateProcessor.processTemplate(MessageTemplate.MSG_VIDEO_INVITE, appt, vars));
                    SmsSender.SendMessage(msg);
                    appt.VideoInviteSent = true;
                    Storage.SaveAppointmentStatus(DateTime.Now.ToString(), appt);
                }
            }
        }

        /// <summary>
        /// This method is called every X minutes to process any changes to the future appointments on the PMS side.
        /// Typically, this covers the next 2 days in the future (not including today, since we don't send the registration message if the appointment is made today)
        /// 
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">The current information from the PMS</param>
        public void ProcessUpcomingAppointments(List<PmsAppointment> stored, List<PmsAppointment> incoming) 
        {
            // pseudo code
            // for each incoming appointment
            //   is it new - send the pre-registration message, and add it to stored
            foreach (var appt in incoming.Where(n => n.PatientMobilePhone != null && IsNearFuture(n.AppointmentStartTime))) // we only send these messages 2-3 days in the future
            {
                if (findApp(stored, appt.AppointmentFhirID) == null) 
                { 
                    SmsMessage msg = new SmsMessage(normalisePhoneNumber(appt.PatientMobilePhone), TemplateProcessor.processTemplate(MessageTemplate.MSG_REGISTRATION, appt, null));
                    SmsSender.SendMessage(msg);
                    appt.PostRegistrationMessageSent = true;
                    Storage.SaveAppointmentStatus(DateTime.Now.ToString(), appt);
                }
            }
        }

        /// <summary>
        /// This method is called every X seconds to process any incoming SMS appointments
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">Sms Messages received since last poll</param>
        public void ProcessIncomingMessages(List<PmsAppointment> appts, List<SmsMessage> incoming)
        {
            foreach (var msg in incoming)
            {
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
                List<PmsAppointment> candidates = findCandidateAppointments(appts, msg.phone);
                if (candidates.Count == 0)
                {
                    handleUnknownMessage(msg);
                }
                else
                {
                    PmsAppointment appt = candidates.Count == 1 ? candidates[0] : chooseRelevantAppointment(candidates, msg);
                    if (appt == null)
                    {
                        processUnexpectedResponse(candidates[0], msg);
                    }
                    else if (appt.VideoInviteSent)
                    {
                        processVideoInviteResponse(appt, msg);
                    }
                    else if (appt.ScreeningMessageSent && !appt.ScreeningMessageResponse)
                    {
                        processScreeningResponse(appt, msg);
                    }
                    else if (appt.ArrivalStatus == AppointmentStatus.Booked)
                    {
                        processArrivalMessage(appt, msg);
                    } 
                    else
                    {
                        processUnexpectedResponse(appt, msg);
                    }

                }
            }
               
        }

        private void handleUnknownMessage(SmsMessage msg)
        {
            // a future possible enhancement is to ask the user which patient the appointment is for; this will smooth the work flow, but the response 
            // processing might be complicated. can it be just a medicare number and date? 
            SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_UNKNOWN_PH, null, null));
            SmsSender.SendMessage(rmsg);
        }

        private void processVideoInviteResponse(PmsAppointment appt, SmsMessage msg)
        {
            if (messageMatches(msg.message, "joined", "ok"))
            {
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_VIDEO_THX, appt, null));
                SmsSender.SendMessage(rmsg);
                appt.ArrivalStatus = AppointmentStatus.Arrived;
                Storage.SaveAppointmentStatus(DateTime.Now.ToString(), appt);
                // TODO: we also have to get the FHIR update to happen... how? 
            }
            else
            {
                // we haven't understood it 
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_VIDEO, appt, null));
                SmsSender.SendMessage(rmsg);
            }
        }
        private void processScreeningResponse(PmsAppointment appt, SmsMessage msg)
        {
            // the patient should respond with "yes" or "no" but they may not bother and just respond with "arrived"
            // of course they might respond with anything else that we can't understand, so we'll explain apologetically if they do
            if (messageMatches(msg.message, "yes"))
            {
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_SCREENING_YES, appt, null));
                SmsSender.SendMessage(rmsg);
                appt.ScreeningMessageResponse = true;
                appt.IsVideoConsultation = true;
                Storage.SaveAppointmentStatus(DateTime.Now.ToString(), appt);
                // TODO: we also have to get the FHIR update to happen... how? 
            }
            else if (messageMatches(msg.message, "no"))
            {
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_SCREENING_NO, appt, null));
                SmsSender.SendMessage(rmsg);
                appt.ScreeningMessageResponse = true;
                appt.IsVideoConsultation = false;
                Storage.SaveAppointmentStatus(DateTime.Now.ToString(), appt);
            }
            else if (messageMatches(msg.message, "arrived", "here"))
            {
                processArrivalMessage(appt, msg);
            }
            else
            {
                // we haven't understood it 
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_SCREENING, appt, null));
                SmsSender.SendMessage(rmsg);
            }
        }

        private void processArrivalMessage(PmsAppointment appt, SmsMessage msg)
        {
            if (messageMatches(msg.message, "arrived", "here"))
            {
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_ARRIVED_THX, appt, null));
                SmsSender.SendMessage(rmsg);
                appt.ScreeningMessageResponse = true;
                appt.ArrivalStatus = AppointmentStatus.Arrived;
                Storage.SaveAppointmentStatus(DateTime.Now.ToString(), appt);
                // TODO: we also have to get the FHIR update to happen... how? 
            }
            else
            {
                // we haven't understood it 
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_ARRIVING, appt, null));
                SmsSender.SendMessage(rmsg);
            }
        }

        private void processUnexpectedResponse(PmsAppointment appt, SmsMessage msg)
        {
            SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_UNEXPECTED, appt, null));
            SmsSender.SendMessage(rmsg);
        }

        private PmsAppointment chooseRelevantAppointment(List<PmsAppointment> candidates, SmsMessage msg)
        {
            if (candidates.Count > 2)
            {
                SmsMessage rmsg = new SmsMessage(msg.phone, TemplateProcessor.processTemplate(MessageTemplate.MSG_TOO_MANY_APPOINTMENTS, candidates[0], null));
                SmsSender.SendMessage(rmsg);
            }
            else
            {
                // pseudo code: 
                // if the two appointments are at the same time, we only care about the first one
                // otherwise, in principle we are interested in the first one, unless the message response belongs to an earlier cycle ("yes" / "no") and we're waiting for that
                var appt1 = candidates[0].ArrivalTime < candidates[1].ArrivalTime ? candidates[0] : candidates[1];
                var appt2 = candidates[0].ArrivalTime < candidates[1].ArrivalTime ? candidates[1] : candidates[0];
                if (messageMatches(msg.message, "yes", "no"))
                {
                    foreach (var appt in candidates)
                    {
                        if (appt.ScreeningMessageSent && !appt.ScreeningMessageResponse)
                        {
                            return appt;
                        }
                    }
                    return null;
                }
                else if (messageMatches(msg.message, "joined"))
                {
                    foreach (var appt in candidates)
                    {
                        if (appt.VideoInviteSent)
                        {
                            return appt;
                        }
                    }
                    return null;
                }
                else if (messageMatches(msg.message, "arrived"))
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
                }
            }
            return null;
        }

        private List<PmsAppointment> findCandidateAppointments(List<PmsAppointment> appts, string phone)
        {
            List<PmsAppointment> list = new List<PmsAppointment>();
            foreach (var appt in appts)
            {
                if (isSamePhone(appt.PatientMobilePhone, phone))
                {
                    list.Add(appt);
                }
            }
            return list;
        }

        private bool messageMatches(string msg, params string[] values)
        {
            foreach (string s in values) {
                if (messageTextMatches(msg, s))
                {
                    return true;
                }
            }
            return false;
        }

        private bool messageTextMatches(string msg, string value)
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

        private bool isSamePhone(string p1, string p2)
        {
            p1 = normalisePhoneNumber(p1);
            p2 = normalisePhoneNumber(p2);
            return p1 == p2;
        }

        private string normalisePhoneNumber(string p1)
        {
            p1 = p1.Replace(" ", "");
            if (p1.StartsWith("04")) {
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

        private String findRoomNote(string practId)
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

        private PmsAppointment findApp(List<PmsAppointment> appointments, string id)
        {
            foreach (PmsAppointment t in appointments)
            {
                if (t.AppointmentFhirID == id)
                {
                    return t;
                }
            }
            return null;
        }

    }
}
