using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Models
{
    public interface IArrivalsLocalStorage
    {
        /// <summary>
        /// Save the Room mappings to storage
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        Task SaveRoomMappings(IEnumerable<DoctorRoomLabelMappings> mappings);

        /// <summary>
        /// Load the room mappings from storage
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DoctorRoomLabelMappings>> LoadRoomMappings();

        /// <summary>
        /// Save the templates to storage
        /// </summary>
        /// <param name="templates"></param>
        /// <returns></returns>
        Task SaveTemplates(IEnumerable<MessageTemplate> templates);

        /// <summary>
        /// Load the templates from storage
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MessageTemplate>> LoadTemplates();

        /// <summary>
        /// Save that this received message was not able to be 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SaveUnprocessableMessage(string date, SmsMessage message);

        /// <summary>
        /// In the event of a system shutdown, re-reading the unprocessed messages
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SmsMessage>> LoadUnprocessableMessages(string date);

        // TODO: Not sure if we actually need to store anything in here, maybe the actual message content that was received.
        // Could this go into a note in the Appointment itself as a write-back instead?
        Task SaveAppointmentStatus(string date, PmsAppointment appt);
        Task LoadAppointmentStatus(string date, PmsAppointment appt);

        #region << Settings >>
        Task<Settings> LoadSettings();
        Task SaveSettings(Settings settings);
        #endregion

        #region << Storage Maintenance Operations >>
        /// <summary>
        /// Cleanup any temporary or historic files
        /// </summary>
        /// <returns></returns>
        Task CleanupHistoricCont();
        #endregion
    }
}
