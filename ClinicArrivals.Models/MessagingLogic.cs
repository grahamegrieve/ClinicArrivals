using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Models
{
    /// <summary>
    ///   This is the core logic of the ClinicalArrivals class; it's where all the processing logic lives
    ///   
    ///   There's 3 different ways that this gets called:
    ///     * every X seconds (typically, 30), processing today's appointments 
    ///     * every 5 minutes, processing the appointments for the next 3 days 
    ///     * every X seconds, processing incoming SMS messages
    ///     
    ///     
    /// </summary>
    class MessagingLogic
    {
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
        public IAppointmentUpdater AppointmentUpdateer { get; set; }

        /// <summary>
        /// Provides template processing services to turn a template + variables into ready to go text
        /// </summary>
        public ITemplateProcessor TemplateProcessor { get; set; }

        /// <summary>
        ///   Logging errors somewhere..
        /// </summary>
        public ILoggingService Logger { get; set; }

        // Call this before using the 
        public void Initialise(Settings settings)
        {

        }

        /// <summary>
        /// This method is called every X seconds to process any changes to the appointments on the PMS side 
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">The current information from the PMS</param>
        public void ProcessTodaysAppointments(List<PmsAppointment> stored, List<PmsAppointment> incoming)
        {

        }

        /// <summary>
        /// This method is called every X minutes to process any changes to the future appointments on the PMS side.
        /// Typically, this the next 2 days in the future (not including this one, since that processing will occur in ProcessTodaysAppointments)
        /// 
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">The current information from the PMS</param>
        public void ProcessUpcomingAppointments(List<PmsAppointment> stored, List<PmsAppointment> incoming) 
        {
        }

        /// <summary>
        /// This method is called every X seconds to process any incoming SMS appointments
        /// </summary>
        /// <param name="stored">The view of the appointments we already had (important, because it remembers what messages we already sent)</param>
        /// <param name="incoming">Sms Messages received since last poll</param>
        public void ProcessIncomingMessages(List<PmsAppointment> stored, List<SmsMessage> incoming)
        {

        }


    }
}
