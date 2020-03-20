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
        /// Save that this received message was not able to be 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SaveUnprocessableMessage(SmsMessage message);

        Task<IEnumerable<SmsMessage>> LoadUnprocessableMessages();

        Task SaveAppointmentStatus(PmsAppointment appt);
        Task LoadAppointmentStatus(PmsAppointment appt);
    }
}
