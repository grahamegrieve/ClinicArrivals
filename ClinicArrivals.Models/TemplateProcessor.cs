using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ClinicArrivals.Models
{
    public class TemplateProcessor
    {
        public ObservableCollection<MessageTemplate> Templates { get; set; }

        public void Initialise(Settings settings)
        {

        }

        /// <summary>
        ///  this is liquid syntax; plan is to replace it with a real liquid processor at some stage.
        ///  The names of the variables are FHIR Based, even though the internal model is not FHIR based
        /// </summary>
        /// <param name="templateId">Use on of the MessageTemplate consts</param>
        /// <param name="appt">Details of the appointment</param>
        /// <param name="vars">Additional information from the engine</param>
        /// <returns></returns>
        public string processTemplate(string templateId, PmsAppointment appt, Dictionary<string, string> vars)
        {
            string source = getTemplate(templateId);
            source = source.Replace("{{Patient.id}}", appt.PatientFhirID); // might be useful internal/debugging
            source = source.Replace("{{Patient.name}}", appt.PatientName);
            source = source.Replace("{{Patient.telecom.mobile}}", appt.PatientMobilePhone);
            source = source.Replace("{{Practitioner.name}}", appt.PractitionerName);
            source = source.Replace("{{Practitioner.id}}", appt.PractitionerFhirID);
            source = source.Replace("{{Appointment.id}}", appt.AppointmentFhirID);
            source = source.Replace("{{Appointment.status}}", appt.ArrivalStatus.ToString());
            source = source.Replace("{{Appointment.start}}", appt.AppointmentStartTime.ToString());
            source = source.Replace("{{Appointment.start.date}}", appt.AppointmentStartTime.ToString("d-MMM"));
            source = source.Replace("{{Appointment.start.time}}", appt.AppointmentStartTime.ToString("hh:mm tt"));
            source = source.Replace("{{Appointment.arrival}}", appt.ArrivalTime == null ? "??" : appt.ArrivalTime.ToString());
            source = source.Replace("{{Appointment.id}}", appt.AppointmentFhirID);
            if (vars != null)
            {
                foreach (var s in vars.Keys)
                {
                    source = source.Replace("{{" + s + "}}", vars[s]);
                }
            }
            return source;
        }

        private string getTemplate(string templateId)
        {
            foreach (var template in Templates)
            {
                if (template.MessageType == templateId)
                {
                    return template.Template;
                }
            }
            return "Unknown template " + templateId;
        }
    }
}