using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Models
{
    public interface IVideoConferenceManager
    {
        void Initialize(Settings settings);

        /// <summary>
        /// Get an internal id for conference
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        VideoCallDetails getConferenceDetails(String appointmentId, Boolean GetItReady);

        /// <summary>
        /// how much notice is appropriate when the 
        /// </summary>
        /// <returns></returns>
        int getNotificationMinutes();

        /// <summary>
        /// Return true if it's possible to know if the patient has joined (not always possible with video services)
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        Boolean canKnowIfJoined();

        /// <summary>
        /// Return true if someone (assumed to be the patient) has joined the conference call
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        Boolean hasSomeoneJoined(String appointmentId);

        /// <summary>
        /// Some video conferencing methods 
        /// </summary>
        /// <returns></returns>
        bool AsksForVideoUrl();
    }

    public class VideoCallDetails
    {
        public string id { get; set; }
        public string url { get; set; }
    }
}
