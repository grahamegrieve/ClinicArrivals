using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinicArrivals.Models
{
    public class MessageProcessing
    {
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

        private static FhirClient GetServerConnection()
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

        ISmsProcessor GetSmsProcessor()
        {
            return new TestSmsProcessor();
        }

        /// <summary>
        /// Check for outgoing messages
        /// If new appts are found, they will be added, missing ones will be removed
        /// (not a flush and add in again - as this will lose data)
        /// </summary>
        /// <param name="model"></param>
        public static async System.Threading.Tasks.Task CheckAppointments(ArrivalsModel model)
        {
            var oldexptecting = model.Expecting.ToList();
            var oldwaiting = model.Waiting.ToList();

            var server = GetServerConnection();
            if (server == null)
                return;

            var criteria = new SearchParams();
            criteria.Add("date", model.DisplayingDate);
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
                PmsAppointment app = null;
                if (entry.Status == Appointment.AppointmentStatus.Booked)
                {
                    app = await ToPmsAppointment(entry, resolveReference);
                    if (app != null && !model.Expecting.Contains(app))
                    {
                        model.Expecting.Add(app);
                    }
                }
                if (entry.Status == Appointment.AppointmentStatus.Arrived)
                {
                    app = await ToPmsAppointment(entry, resolveReference);
                    if (app != null && !model.Waiting.Contains(app))
                    {
                        model.Waiting.Add(app);
                    }
                }
                if(entry.Status == Appointment.AppointmentStatus.Fulfilled)
                {
                    if(oldwaiting.Select(x => x.AppointmentFhirID).Contains(entry.Id))
                    {
                        app = await ToPmsAppointment(entry, resolveReference);
                        OnVisitStarted?.BeginInvoke(app, null, null);
                    }
                }

                if (app != null)
                {
                    // Check if the practitioner has a mapping already
                    if (!model.RoomMappings.Any(m => m.PractitionerFhirID == app.PractitionerFhirID))
                    {
                        // Add in an empty room mapping
                        model.RoomMappings.Add(new DoctorRoomLabelMapping()
                        {
                            PractitionerFhirID = app.PractitionerFhirID,
                            PractitionerName = app.PractitionerName
                        });
                    }
                }
            }

            // finished processing all the items, so remove any that are left
            foreach (var item in oldexptecting)
                model.Expecting.Remove(item);
            foreach (var item in oldwaiting)
                model.Waiting.Remove(item);
        }

        public string StandardiseMobileNumber(string mobile)
        {
            // Simply clear the values
            return mobile?.Replace(" ", "");
        }

        public async System.Threading.Tasks.Task CheckForInboundSmsMessages(ArrivalsModel model)
        {
            ISmsProcessor sms = GetSmsProcessor();
            // sms.Initialize();
            // TODO: Thread this into the background, but report errors back to the status screen
            // TODO: If this needs paging or something, repeat this call till its complete, or similar
            var messages = await sms.ReceiveMessages();
            foreach (var item in messages)
            {
                var appts = MatchPatient(model, item.phone);
                foreach (var appt in appts)
                {
                    if (ActionForMessage(item.message) == "arrived")
                    {
                        await ArriveAppointment(appt);
                        appt.LastPatientMessage = item.message;
                    }
                }
                if (!appts.Any())
                {
                    item.date = DateTimeOffset.Now.ToString();
                    model.UnprocessableMessages.Add(item);
                }
            }
        }

        public string ActionForMessage(string message)
        {
            string cleansedMessage = message?.ToLower()?.Trim();
            if (cleansedMessage == "arrived")
                return "arrived";
            return "unknown";
        }

        public IEnumerable<PmsAppointment> MatchPatient(ArrivalsModel model, string mobile)
        {
            if (String.IsNullOrEmpty(StandardiseMobileNumber(mobile)))
                return null;
            return model.Expecting.Where(a => StandardiseMobileNumber(a.PatientMobilePhone) == StandardiseMobileNumber(mobile))
                .Union(model.Waiting.Where(a => StandardiseMobileNumber(a.PatientMobilePhone) == StandardiseMobileNumber(mobile)));
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

        // Check for incoming messages

        // Handle arrived message
        public async static System.Threading.Tasks.Task ArriveAppointment(PmsAppointment appt)
        {
            var server = GetServerConnection();
            if (server == null)
                return;

            Appointment appointment = await server.ReadAsync<Appointment>($"{server}/Appointment/{appt.AppointmentFhirID}");
            if (appointment.Status != Appointment.AppointmentStatus.Arrived)
            {
                appointment.Status = Appointment.AppointmentStatus.Arrived; // this means they've arrived (happy that they stay out there)
                server.Update(appointment);
            }

            //appointment.Status = Appointment.AppointmentStatus.Booked; // not turned up yet
            //appointment.Status = Appointment.AppointmentStatus.Arrived; // this means they've arrived (happy that they stay out there)
            //appointment.Status = Appointment.AppointmentStatus.Fulfilled; // seeing the practitioner
            //appointment.Status = Appointment.AppointmentStatus.Cancelled; // duh
        }

        // Handle 
    }


}
