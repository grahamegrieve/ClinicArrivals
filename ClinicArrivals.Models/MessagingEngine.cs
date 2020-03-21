using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public ITemplateProcessor TemplateProcessor { get; set; }

        /// <summary>
        /// Cconnects to the VideoConferencing engine
        /// </summary>
        public IVideoConferenceManager VideoManager { get; set; }

        /// <summary>
        ///   Logging errors somewhere..
        /// </summary>
        public ILoggingService Logger { get; set; }

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
            foreach (var napp in incoming)
            {
                if (napp.PatientMobilePhone != null)
                {
//                    var oapp = findApp(stored, napp);
//                    if (oapp == null)
//                    {
//                        processNewFutureAppointment(napp);
//                    }
                }
            }
        }

        /// <summary>
        /// This method is called every X seconds to process any incoming SMS appointments
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">Sms Messages received since last poll</param>
        public void ProcessIncomingMessages(List<PmsAppointment> stored, List<SmsMessage> incoming)
        {
            // pseudo code 
            // find the candidate appointments for this mobile phone 
            // if there aren't any - return the 'please call reception message', and drop this message
            // if there's more than one, pick one
            // ok, now we have appointment and message
            // if we sent an invitation for a video conference 
            //   process as a response to the invitation
            // else if we are expecting them to arrive 
            //   process as an arrival messages
            // else if we are expecting a response to the screening
            //   process as a response to the screening
            // else
            //   we are not expecting a response - send message explaining that 
        }
    }
}
