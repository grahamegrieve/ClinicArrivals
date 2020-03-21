using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Hl7.Fhir.Model.Appointment;

namespace Test.Models
{
    // all Tests are set to happen on the arbitrary date of 1-1-21
    // the time of execution varies depending on the type of test that has being done
    [TestClass]
    public class MessageEngineTester
    {
        private readonly List<SmsMessage> OutputMsgs = new List<SmsMessage>();
        private readonly List<StorageOp> StorageOps = new List<StorageOp>();

        [TestMethod]
        public void testPreRegistrationMessageSent()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 9, 0, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            // set it up:
            nl.Add(tomorrowsAppointment());
            // run it
            engine.ProcessUpcomingAppointments(new List<PmsAppointment>(), nl);
            // inspect outputs:
            Assert.IsTrue(OutputMsgs.Count == 2);
            Assert.IsTrue(OutputMsgs[0].phone == "+0411012345");
            Assert.IsTrue(OutputMsgs[0].message == "Patient Test Patient #1 has an appointment with Dr Adam Ant at 9:15am on 2-Jan 2021");
            Assert.IsTrue(OutputMsgs[1].phone == "+0411012345");
            Assert.IsTrue(OutputMsgs[2].message == "3 hours prior to the appointment, you will be sent a COVID-19 screening check to decide whether you should do a video consultation rather than seeing the doctor in person");
            Assert.IsTrue(StorageOps.Count == 1);
            Assert.IsTrue(StorageOps[0].Appointment.AppointmentFhirID == "1234");
            Assert.IsTrue(StorageOps[0].Appointment.PostRegistrationMessageSent);
        }

        // test case generation
        private PmsAppointment tomorrowsAppointment()
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient #1";
            app.PatientMobilePhone = "+0411012345";
            app.PractitionerName = "Dr Adam Ant";
            app.PractitionerFhirID = "p123";
            app.AppointmentFhirID = "1234";
            app.ArrivalStatus = AppointmentStatus.Pending;
            app.AppointmentStartTime = new DateTime(2021, 1, 2, 9, 15, 0);
            return app;
        }

        // supporting infrastructure
        private Settings testSettings()
        {
            return new Settings();
        }

        private MessagingEngine makeEngine()
        {
            MessagingEngine engine = new MessagingEngine();
            engine.Initialise(testSettings());
            engine.SmsSender = new MessageLogicTesterSmsHandler(this);
            engine.Storage = new MessageLogicTesterStorageHandler(this);
            return engine;
        }
        private void reset()
        {
            OutputMsgs.Clear();
            StorageOps.Clear();
        }

        private class MessageLogicTesterSmsHandler : ISmsProcessor
        {
            private MessageEngineTester owner;

            public MessageLogicTesterSmsHandler(MessageEngineTester messageLogicTester)
            {
                this.owner = messageLogicTester;
            }

            public void Initialize(Settings settings)
            {
            }

            public Task<IEnumerable<SmsMessage>> ReceiveMessages()
            {
                return null; // we don't actually use that
            }

            public void SendMessage(SmsMessage sendMessage)
            {
                owner.OutputMsgs.Add(sendMessage);
            }
        }

        private class MessageLogicTesterStorageHandler : IArrivalsLocalStorage
        {
            private MessageEngineTester owner;

            public MessageLogicTesterStorageHandler(MessageEngineTester messageLogicTester)
            {
                this.owner = messageLogicTester;
            }

            public Task CleanupHistoricCont()
            {
                throw new NotImplementedException();
            }

            public Task LoadAppointmentStatus(string date, PmsAppointment appt)
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<DoctorRoomLabelMappings>> LoadRoomMappings()
            {
                throw new NotImplementedException();
            }

            public Task<Settings> LoadSettings()
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<SmsMessage>> LoadUnprocessableMessages(string date)
            {
                throw new NotImplementedException();
            }

            public Task SaveAppointmentStatus(string date, PmsAppointment appt)
            {
                throw new NotImplementedException();
            }

            public Task SaveRoomMappings(IEnumerable<DoctorRoomLabelMappings> mappings)
            {
                throw new NotImplementedException();
            }

            public Task SaveSettings(Settings settings)
            {
                throw new NotImplementedException();
            }

            public Task SaveUnprocessableMessage(string date, SmsMessage message)
            {
                throw new NotImplementedException();
            }
        }
        private class StorageOp
        {
            public PmsAppointment Appointment { get; set; }
        }
    }

}
