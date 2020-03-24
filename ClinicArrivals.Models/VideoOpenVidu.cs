using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Models
{
    public class VideoOpenVidu : IVideoConferenceManager
    {
        private Guid systemId;
        private string secret;

        public void Initialize(Settings settings)
        {
            systemId = settings.SystemIdentifier;
            secret = settings.OpenViduSecret;
        }

        /// <summary>
        /// Get URL for conference
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        public VideoCallDetails getConferenceDetails(String appointmentId, Boolean GetItReady)
        {
            var name = systemId.ToString() + "-" + appointmentId;
            var client = new OpenViduClient("https://video.healthinterections.com.au", secret);
            VideoCallDetails ret = new VideoCallDetails();
            ret.id = client.SetUpSession();
            ret.url = "https://video.healthinterections.com.au/openvidu-call/#/" + name;
            return ret;
        }

        /// <summary>
        /// Return true if it's possible to know if the patient has joined (not always possible with video services)
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        public Boolean canKnowIfJoined()
        {
            return false;
        }

        /// <summary>
        /// Return true if someone (assumed to be the patient) has joined the conference call
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        public Boolean hasSomeoneJoined(String appointmentId)
        {
            return false;
        }


        public int getNotificationMinutes()
        {
            return 10;
        }


        public bool AsksForVideoUrl()
        {
            return false;
        }

    }
}
