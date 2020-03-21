using ClinicArrivals.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals
{
    public class Model : ArrivalsModel
    {
        public Model()
        {
#if DEBUG
            // This is some test data to assist in the UI Designer
            Expecting.Add(new PmsAppointment() { ArrivalTime = "10:35am", PatientName = "Postlethwaite, Brian", PatientMobilePhone = "0423857505", ArrivalStatus = "Pending", AppointmentStartTime = "9:45am", PractitionerName = "Dr Nathan Pinskier" });
            Expecting.Add(new PmsAppointment() { ArrivalTime = "10:35am", PatientName = "Esler, Brett", PatientMobilePhone = "0423083847", ArrivalStatus = "Pending", AppointmentStartTime = "10:00am", PractitionerName = "Dr Nathan Pinskier" });
            Waiting.Add(new PmsAppointment() { ArrivalTime = "10:35am", PatientName = "Grieve, Grahame", PatientMobilePhone = "0423083847", ArrivalStatus = "Arrived", AppointmentStartTime = "10:00am", PractitionerName = "Dr Nathan Pinskier" });
            RoomMappings.Add(new DoctorRoomLabelMapping()
            {
                PractitionerFhirID = "1",
                PractitionerName = "Dr Nathan",
                LocationName = "Room 1",
                LocationDescription = "Proceed through the main lobby and its the 3rd door on the right"
            });
            RoomMappings.Add(new DoctorRoomLabelMapping()
            {
                PractitionerFhirID = "1",
                PractitionerName = "Dr Smith",
                LocationName = "Room 2",
                LocationDescription = "Proceed through the main lobby and its the 2nd door on the right"
            });
            UnprocessableMessages.Add(new SmsMessage("+61 432 857505", "test") { date= "21-3-2020 12:40pm" });
            UnprocessableMessages.Add(new SmsMessage("+61 432 857505", "test 2") { date = "21-3-2020 1:15pm" });
            UnprocessableMessages.Add(new SmsMessage("+61 432 857505", "test 3") { date = "21-3-2020 1:23pm" });

            Templates.Add(new MessageTemplate("Registration", "Thank you making an appointment to see Dr X. [X] hours before the appointment, we will send you an SMS asking with you meet the criteria documented at http://www.rcpa.org.au/xxx, to decide whether you will talk to the doctor by telephone video, or physically come to the clinic. Please respond to this message to confirm you have seen it (or your appointment will be canceled"));
            Templates.Add(new MessageTemplate("Cancellation")); // no message means no sending
            Templates.Add(new MessageTemplate("ConsiderTeleHealth", "Please consult the web page http://www.rcpa.org.au/xxx to determine whether you are eligable to meet with the doctor by phone/video. If you are, respond to this message with YES otherwise respond with NO")); // no message means no sending
            Templates.Add(new MessageTemplate("PreApppointment", "Due to the COVID-19 Pandemic, this clinic has closed it's waiting room. Please wait in your car, and SMS \"arrived\" to [phone number]"));
            Templates.Add(new MessageTemplate("UnknownPatient", "Your mobile phone number is not registered with the clinic, please call reception to confirm your details"));
            Templates.Add(new MessageTemplate("DoctorReady", "The doctor is ready for you now. [Room Mapping Notes]"));
#endif
            Storage = new ArrivalsFileSystemStorage();
            Settings.Save = new SaveSettingsCommand(Storage);
            Settings.Reload = new ReloadSettingsCommand(Storage);
            SaveRoomMappings = new SaveRoomMappingsCommand(Storage);
            ReloadRoomMappings = new ReloadRoomMappingsCommand(Storage);
            SaveTemplates = new SaveTemplatesCommand(Storage);
            ReloadTemplates = new ReloadTemplatesCommand(Storage);
        }
    }
}
