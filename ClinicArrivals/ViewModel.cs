using ClinicArrivals.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ClinicArrivals
{
    public class ViewModel : ArrivalsModel
    {
        public string Text { get { return System.IO.File.ReadAllText("about.md"); }  }

        public BackgroundProcess ReadSmsMessage;
        public BackgroundProcess ScanAppointments;

        public ViewModel()
        {
            CreateTestDataForDebug();

            // Assign all of the implementations for the interfaces
            Storage = new ArrivalsFileSystemStorage();
            Settings.Save = new SaveSettingsCommand(Storage);
            Settings.Reload = new ReloadSettingsCommand(Storage);
            SaveRoomMappings = new SaveRoomMappingsCommand(Storage);
            ReloadRoomMappings = new ReloadRoomMappingsCommand(Storage);
            SaveTemplates = new SaveTemplatesCommand(Storage);
            ReloadTemplates = new ReloadTemplatesCommand(Storage);
        }

        public async Task Initialize(Dispatcher dispatcher)
        {
            // read the settings from storage
            Settings.CopyFrom(await Storage.LoadSettings());
            if (Settings.SystemIdentifier == Guid.Empty)
            {
                // this only occurs when the system hasn't had one allocated
                // so we can create a new one, then save the settings.
                // (this will force an empty setting file with the System Identifier if needed)
                Settings.AllocateNewSystemIdentifier();
                await Storage.SaveSettings(Settings);
            }

            // read the room mappings from storage
            RoomMappings.Clear();
            foreach (var map in await Storage.LoadRoomMappings())
                RoomMappings.Add(map);

            // reload any unmatched messages
            var messages = await Storage.LoadUnprocessableMessages(DisplayingDate);
            foreach (var item in messages)
                UnprocessableMessages.Add(item);

            // setup the background worker routines
            ReadSmsMessage = new BackgroundProcess(Settings, serverStatuses.IncomingSmsReader, dispatcher, async () =>
            {
                // Logic to run on this process
                // (called every settings.interval)
                StatusBarMessage = $"Last read SMS messages at {DateTime.Now.ToLongTimeString()}";
                var processor = new MessageProcessing();
                await processor.CheckForInboundSmsMessages(this);
                // return System.Threading.Tasks.Task.CompletedTask;
            });

            ScanAppointments = new BackgroundProcess(Settings, serverStatuses.AppointmentScanner, dispatcher, async () =>
            {
                // Logic to run on this process
                // (called every settings.interval)
                await MessageProcessing.CheckAppointments(this);
                // return System.Threading.Tasks.Task.CompletedTask;
            });
        }

        private void CreateTestDataForDebug()
        {
#if DEBUG
            // This is some test data to assist in the UI Designer
            Expecting.Add(new PmsAppointment() { ArrivalTime = DateTime.Parse("10:35am"), PatientName = "Postlethwaite, Brian", PatientMobilePhone = "0423857505", ArrivalStatus = Hl7.Fhir.Model.Appointment.AppointmentStatus.Pending, AppointmentStartTime = DateTime.Parse("9:45am"), PractitionerName = "Dr Nathan Pinskier" });
            Expecting.Add(new PmsAppointment() { ArrivalTime = DateTime.Parse("10:35am"), PatientName = "Esler, Brett", PatientMobilePhone = "0423083847", ArrivalStatus = Hl7.Fhir.Model.Appointment.AppointmentStatus.Pending, AppointmentStartTime = DateTime.Parse("10:00am"), PractitionerName = "Dr Nathan Pinskier" });
            Waiting.Add(new PmsAppointment() { ArrivalTime = DateTime.Parse("10:35am"), PatientName = "Grieve, Grahame", PatientMobilePhone = "0423083847", ArrivalStatus = Hl7.Fhir.Model.Appointment.AppointmentStatus.Arrived, AppointmentStartTime = DateTime.Parse("10:00am"), PractitionerName = "Dr Nathan Pinskier" });
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
            UnprocessableMessages.Add(new SmsMessage("+61 432 857505", "test") { date = "21-3-2020 12:40pm" });
            UnprocessableMessages.Add(new SmsMessage("+61 432 857505", "test 2") { date = "21-3-2020 1:15pm" });
            UnprocessableMessages.Add(new SmsMessage("+61 432 857505", "test 3") { date = "21-3-2020 1:23pm" });

            Templates.Add(new MessageTemplate(MessageTemplate.MSG_REGISTRATION, "Thank you making an appointment to see Dr X. [X] hours before the appointment, we will send you an SMS asking with you meet the criteria documented at http://www.rcpa.org.au/xxx, to decide whether you will talk to the doctor by telephone video, or physically come to the clinic. Please respond to this message to confirm you have seen it (or your appointment will be canceled"));
            Templates.Add(new MessageTemplate(MessageTemplate.MSG_CANCELLATION)); // no message means no sending
            Templates.Add(new MessageTemplate(MessageTemplate.MSG_SCREENING, "Please consult the web page http://www.rcpa.org.au/xxx to determine whether you are eligable to meet with the doctor by phone/video. If you are, respond to this message with YES otherwise respond with NO")); // no message means no sending
            Templates.Add(new MessageTemplate(MessageTemplate.MSG_PREAPP, "Due to the COVID-19 Pandemic, this clinic has closed it's waiting room. Please wait in your car, and SMS \"arrived\" to [phone number]"));
            Templates.Add(new MessageTemplate(MessageTemplate.MSG_UNKNOWN_PAT, "Your mobile phone number is not registered with the clinic, please call reception to confirm your details"));
            Templates.Add(new MessageTemplate(MessageTemplate.MSG_APPT_READY, "The doctor is ready for you now. [Room Mapping Notes]"));
#endif
        }
    }
}
