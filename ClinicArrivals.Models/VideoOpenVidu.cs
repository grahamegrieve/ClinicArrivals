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

        private List<ScheduledSession> sessions = new List<ScheduledSession>();
        public void Initialize(Settings settings)
        {
            systemId = settings.SystemIdentifier;
            secret = settings.OpenViduSecret;
        }

        /// <summary>
        /// Get URL for conference
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        public String getConferenceDetails(PmsAppointment appointment, Boolean GetItReady)
        {
            if (String.IsNullOrEmpty(appointment.ExternalData.VideoSessionId))
            {
                var client = new OpenViduClient("https://video.healthintersections.com.au", secret);
                appointment.ExternalData.VideoSessionId = client.SetUpSession();
                sessions.Add(new ScheduledSession() { Start = appointment.AppointmentStartTime, Id = appointment.ExternalData.VideoSessionId });
            }
            return "https://video.healthintersections.com.au/#" + appointment.ExternalData.VideoSessionId; 
        }

        /// <summary>
        /// Return true if it's possible to know if the patient has joined (not always possible with video services)
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        public Boolean canKnowIfJoined()
        {
            return true;
        }

        /// <summary>
        /// Return true if someone (assumed to be the patient) has joined the conference call
        /// </summary>
        /// <param name="id">The id of the appointment (unique ==> Appointment Resource id)</param>
        public Boolean hasSomeoneJoined(String VideoId)
        {
            var client = new OpenViduClient("https://video.healthintersections.com.au", secret);
            return client.hasAnyoneJoined(VideoId);


        }


        public int getNotificationMinutes()
        {
            return 10;
        }


        public bool AsksForVideoUrl()
        {
            return false;
        }

        public void cleanUp()
        {
            var client = new OpenViduClient("https://video.healthintersections.com.au", secret);
            foreach (ScheduledSession ss in sessions) 
            {
                if (ss.Start.AddHours(1) < DateTime.Now)
                {
                    client.EndSession(ss.Id);
                }
            }
        }
    }

    internal class ScheduledSession
    {
        public DateTime Start { get; set; }
        public string Id { get; set; }
    }
}
