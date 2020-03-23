using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinicArrivals.Models
{
    public class FhirAppointmentReader : IFhirAppointmentReader
    {
        public FhirAppointmentReader(Func<FhirClient> GetFhirClient)
        {
            this.GetFhirClient = GetFhirClient;
        }
        private Func<FhirClient> GetFhirClient;

        #region << Oridashi Server Configuration and Management >>
        public delegate void StartedServer();
        public delegate void StoppedServer();
        public delegate void VisitStarted(PmsAppointment appt);

        private static ClinicArrivals.Server.Server server = null;

        public static event StartedServer OnStarted;
        public static event StoppedServer OnStopped;
        public static event VisitStarted OnVisitStarted;

        public static void StartServer(bool UseExamples)
        {
            if (server == null)
            {
                server = new Server.Server();
                server.OnStarted += Server_OnStarted;
                server.OnStopped += Server_OnStopped;
                server.Start(UseExamples);
            }
        }

        public static async System.Threading.Tasks.Task StopServer()
        {
            if (server != null)
            {
                await server.Stop();
                server.OnStarted -= Server_OnStarted;
                server.OnStopped -= Server_OnStopped;
                server = null;
            }
        }

        private static void Server_OnStopped()
        {
            OnStopped?.Invoke();
        }

        private static void Server_OnStarted()
        {
            OnStarted?.Invoke();
        }
        #endregion

        public static FhirClient GetServerConnection()
        {
            if (server?.IsRunning == true)
            {
                FhirClient client = new FhirClient(server.Url, false);   // "http://demo.oridashi.com.au:8304"
                client.OnBeforeRequest += Client_OnBeforeRequest;
                return client;
            }

            return null;
        }

        private static void Client_OnBeforeRequest(object sender, BeforeRequestEventArgs e)
        {
            if (server?.IsRunning == true)
                e.RawRequest.Headers.Add(System.Net.HttpRequestHeader.Authorization, server.Token);
        }

        /// <summary>
        /// Retrieve the set of appointments that have mobile numbers with a status of booked, arrived, or fulfilled
        /// And attach the processing status data from local storage too
        /// </summary>
        /// <param name="model"></param>
        public async System.Threading.Tasks.Task<List<PmsAppointment>> SearchAppointments(DateTime date, IList<DoctorRoomLabelMapping> roomMappings, IArrivalsLocalStorage storage)
        {
            List<PmsAppointment> results = new List<PmsAppointment>();

            var server = GetServerConnection();
            if (server == null)
                return null;

            var criteria = new SearchParams();
            criteria.Add("date", date.Date.ToString("yyyy-MM-dd"));
            criteria.Include.Add("Appointment:actor");
            var bundle = await server.SearchAsync<Appointment>(criteria);

            // Debugging
            var doc = System.Xml.Linq.XDocument.Parse(new Hl7.Fhir.Serialization.FhirXmlSerializer().SerializeToString(bundle));
            Console.WriteLine(doc.ToString(System.Xml.Linq.SaveOptions.None));

            Func<ResourceReference, System.Threading.Tasks.Task<Resource>> resolveReference = async (reference) =>
            {
                if (string.IsNullOrEmpty(reference.Reference))
                    return null;
                ResourceIdentity ri = new ResourceIdentity(reference.Reference);
                var resource = bundle.Entry.FirstOrDefault(e => e.Resource.ResourceType.GetLiteral() == ri.ResourceType && e.Resource.Id == ri.Id)?.Resource;
                if (resource == null)
                {
                    // wasn't returned in the bundle, so go searching for it
                    resource = await server.ReadAsync<Resource>(reference.Reference);
                }
                return resource;
            };

            foreach (var entry in bundle.Entry.Select(e => e.Resource as Appointment).Where(e => e != null))
            {
                PmsAppointment appt = null;
                if (entry.Status == Appointment.AppointmentStatus.Booked 
                    || entry.Status == Appointment.AppointmentStatus.Arrived
                    || entry.Status == Appointment.AppointmentStatus.Fulfilled)
                {
                    appt = await ToPmsAppointment(entry, resolveReference);
                    if (appt != null)
                    {
                        if (!string.IsNullOrEmpty(appt.PatientMobilePhone))
                        {
                            results.Add(appt);

                            // Check if the practitioner has a mapping already
                            if (!roomMappings.Any(m => m.PractitionerFhirID == appt.PractitionerFhirID))
                            {
                                // Add in an empty room mapping
                                roomMappings.Add(new DoctorRoomLabelMapping()
                                {
                                    PractitionerFhirID = appt.PractitionerFhirID,
                                    PractitionerName = appt.PractitionerName
                                });
                            }

                            // And read in the extended content from storage
                            await storage.LoadAppointmentStatus(date, appt);
                        }
                    }
                }
            }
            return results;
        }

        private static async System.Threading.Tasks.Task<PmsAppointment> ToPmsAppointment(Appointment entry, Func<ResourceReference, System.Threading.Tasks.Task<Resource>> resolveReference)
        {
            PmsAppointment appt = new PmsAppointment();
            appt.AppointmentFhirID = entry.Id;
            appt.AppointmentStartTime = entry.Start.Value.DateTime;
            appt.ArrivalStatus = entry.Status.Value;
            appt.PatientFhirID = entry.Participant.FirstOrDefault(p => p.Actor.Reference?.StartsWith("Patient") == true)?.Actor?.Reference;
            appt.PractitionerFhirID = entry.Participant.FirstOrDefault(p => p.Actor.Reference?.StartsWith("Practitioner") == true)?.Actor?.Reference;

            if (string.IsNullOrEmpty(appt.PatientFhirID))
                return null;
            var patient = await resolveReference(new ResourceReference(appt.PatientFhirID));
            if (patient is Patient pat)
            {
                appt.PatientName = pat.Name.Select(n => $"{n.Family}, {String.Join(" ", n.Given)}").FirstOrDefault();
                // The PMS doesn't have enough detail to differentiate into sms usage
                appt.PatientMobilePhone = pat.Telecom.FirstOrDefault(t => t.Use == ContactPoint.ContactPointUse.Mobile)?.Value;
            }
            var practitioner = await resolveReference(new ResourceReference(appt.PractitionerFhirID));
            if (practitioner is Practitioner prac)
            {
                appt.PractitionerName = prac.Name.Select(n => $"{n.Family}, {String.Join(" ", n.Given)}").FirstOrDefault();
            }

            return appt;
        }
    }
}
