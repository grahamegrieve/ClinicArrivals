using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly List<FhirUpdateOp> FhirUpdateOps = new List<FhirUpdateOp>();

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
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+0411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Patient Test Patient #1 has an appointment with Dr Adam Ant at 09:15 AM on 2-Jan. 3 hours prior to the appointment, you will be sent a COVID-19 screening check to decide whether you should do a video consultation rather than seeing the doctor in person", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("save-appt", StorageOps[0].type);
            Assert.AreEqual("1234", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.ExternalData.PostRegistrationMessageSent);
        }

        [TestMethod]
        public void testPreRegistrationMessageNotSent()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 9, 0, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            nl.Add(tomorrowsAppointment());
            sl.Add(tomorrowsAppointment());
            // run it
            engine.ProcessUpcomingAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testPreRegistrationMessageNotSentForToday()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 9, 0, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            // run it
            engine.ProcessUpcomingAppointments(new List<PmsAppointment>(), nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testNothingToDo()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 10, 55, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt2pm());
            sl.Add(appt10am());
            sl.Add(appt2pm());
            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testScreeningMsg()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 10, 55, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Please consult the web page http://www.rcpa.org.au/xxx to determine whether you are eligible to meet with the doctor by phone/video. If you are, respond to this message with YES otherwise respond with NO", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("1002", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.ExternalData.ScreeningMessageSent);
        }

        [TestMethod]
        public void testScreeningMsgDone()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 10, 56, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            sl[1].ExternalData.ScreeningMessageSent = true;
            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testVideoInviteNotYet()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 45, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            sl[1].ExternalData.ScreeningMessageSent = true;
            sl[1].IsVideoConsultation = true;

            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testVideoInviteNow()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 51, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            sl[1].ExternalData.ScreeningMessageSent = true;
            nl[1].IsVideoConsultation = true;

            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Please start your video call at https://meet.jit.si/:guid:-1002. When you have started it, reply to this message with the word \"joined\"", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("1002", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.ExternalData.VideoInviteSent);
        }

        [TestMethod]
        public void testVideoInviteDone()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 51, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            sl[1].ExternalData.ScreeningMessageSent = true;
            sl[1].ExternalData.VideoInviteSent = true;
            nl[1].IsVideoConsultation = true;

            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }


        [TestMethod]
        public void testVideoApptReady()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 13, 1, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            sl[1].ExternalData.ScreeningMessageSent = true;
            sl[1].ArrivalStatus = AppointmentStatus.Arrived;
            sl[1].ExternalData.ArrivalStatus = AppointmentStatus.Arrived;
            nl[1].ArrivalStatus = AppointmentStatus.Fulfilled;
            nl[1].ExternalData.ArrivalStatus = AppointmentStatus.Arrived;

            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("The doctor is ready to see you now. Please go to room 7.", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("1002", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.ArrivalStatus == AppointmentStatus.Fulfilled);
            Assert.IsTrue(StorageOps[0].Appointment.ExternalData.ArrivalStatus == AppointmentStatus.Fulfilled);
        }

        [TestMethod]
        public void testVideoApptReadyDone()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 13, 2, 0);
            reset();
            List<PmsAppointment> nl = new List<PmsAppointment>();
            List<PmsAppointment> sl = new List<PmsAppointment>();
            // set it up:
            nl.Add(appt10am());
            nl.Add(appt1pm());
            sl.Add(appt10am());
            sl.Add(appt1pm());
            sl[1].ExternalData.ScreeningMessageSent = true;
            sl[1].ExternalData.ScreeningMessageSent = true;
            sl[1].ArrivalStatus = AppointmentStatus.Fulfilled;
            nl[1].ArrivalStatus = AppointmentStatus.Fulfilled;

            // run it
            engine.ProcessTodaysAppointments(sl, nl);
            // inspect outputs:
            Assert.AreEqual(0, OutputMsgs.Count);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testIncomingUnkPhone()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 13, 0, 0);
            reset();
            List<PmsAppointment> appts = new List<PmsAppointment>();
            List<SmsMessage> msgs = new List<SmsMessage>();
            // set it up:
            appts.Add(appt10am());
            appts.Add(appt1pm());

            msgs.Add(new SmsMessage("+61411012346", "Arrived"));

            // run it
            engine.ProcessIncomingMessages(appts, msgs);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012346", OutputMsgs[0].phone);
            Assert.AreEqual("This phone number is not associated with an appointment to see the doctor today. Please phone {num} for help", OutputMsgs[0].message);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testScreeningResponseYes()
        {
            MessagingEngine engine = makeEngine();
            var fhirServerUpdater = new MessageLogicFhirUpdaterHandler(this);
            engine.AppointmentUpdater = fhirServerUpdater;
            engine.TimeNow = new DateTime(2021, 1, 1, 13, 0, 0);
            reset();
            List<PmsAppointment> appts = new List<PmsAppointment>();
            List<SmsMessage> msgs = new List<SmsMessage>();
            // set it up:
            appts.Add(appt10am());
            appts.Add(appt1pm());
            appts[1].ExternalData.ScreeningMessageSent = true;

            msgs.Add(new SmsMessage("+61411012345", "Yes"));

            // run it
            engine.ProcessIncomingMessages(appts, msgs);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Thank you. Do not come to the doctor's clinic. You will get an SMS message containing the URL for your video meeting a few minutes before your appointment. You can join from any computer or smartphone", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("1002", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.ExternalData.ScreeningMessageSent == true);
            Assert.IsTrue(StorageOps[0].Appointment.ExternalData.ScreeningMessageResponse == true);
            Assert.IsTrue(StorageOps[0].Appointment.IsVideoConsultation == true);
            Assert.AreEqual("save-video-meeting", FhirUpdateOps[0].type);
            Assert.IsNotNull(FhirUpdateOps[0].comment);
        }

        [TestMethod]
        public void testScreeningResponseNo()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 13, 0, 0);
            reset();
            List<PmsAppointment> appts = new List<PmsAppointment>();
            List<SmsMessage> msgs = new List<SmsMessage>();
            // set it up:
            appts.Add(appt10am());
            appts.Add(appt1pm());
            appts[1].ExternalData.ScreeningMessageSent = true;

            msgs.Add(new SmsMessage("+61411012345", "NO!"));

            // run it
            engine.ProcessIncomingMessages(appts, msgs);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Thank you. When you arrive at the clinic, stay in your car (or outside) and reply \"arrived\" to this message", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("1002", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.ExternalData.ScreeningMessageSent == true);
            Assert.IsTrue(StorageOps[0].Appointment.ExternalData.ScreeningMessageResponse == true);
            Assert.IsTrue(StorageOps[0].Appointment.IsVideoConsultation == false);
        }

        [TestMethod]
        public void testScreeningResponseDuh()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 13, 0, 0);
            reset();
            List<PmsAppointment> appts = new List<PmsAppointment>();
            List<SmsMessage> msgs = new List<SmsMessage>();
            // set it up:
            appts.Add(appt10am());
            appts.Add(appt1pm());
            appts[1].ExternalData.ScreeningMessageSent = true;

            msgs.Add(new SmsMessage("+61411012345", "what?!"));

            // run it
            engine.ProcessIncomingMessages(appts, msgs);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("The robot processing this message is stupid, and didn't understand your response. Please answer yes or no, or phone {num} for help", OutputMsgs[0].message);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testArrived()
        {
            MessagingEngine engine = makeEngine();
            var fhirServerUpdater = new MessageLogicFhirUpdaterHandler(this);
            engine.AppointmentUpdater = fhirServerUpdater;
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 58, 0);
            reset();
            List<PmsAppointment> appts = new List<PmsAppointment>();
            List<SmsMessage> msgs = new List<SmsMessage>();
            // set it up:
            appts.Add(appt10am());
            appts.Add(appt1pm());
            appts[1].ExternalData.ScreeningMessageSent = true;
            appts[1].ExternalData.ScreeningMessageResponse = true;

            msgs.Add(new SmsMessage("+61411012345", "ARRIVED"));

            // run it
            engine.ProcessIncomingMessages(appts, msgs);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Thanks for letting us know that you're here. We'll let you know as soon as the doctor is ready for you", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("1002", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.AreEqual("save-appt", StorageOps[0].type);
            Assert.AreEqual(AppointmentStatus.Arrived, StorageOps[0].Appointment.ArrivalStatus);
            Assert.AreEqual(AppointmentStatus.Arrived, StorageOps[0].Appointment.ExternalData.ArrivalStatus);
            Assert.AreEqual("save-status", FhirUpdateOps[0].type);
            Assert.AreEqual(AppointmentStatus.Arrived, FhirUpdateOps[0].status);
        }

        [TestMethod]
        public void testArrivedDuh()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 58, 0);
            reset();
            List<PmsAppointment> appts = new List<PmsAppointment>();
            List<SmsMessage> msgs = new List<SmsMessage>();
            // set it up:
            appts.Add(appt10am());
            appts.Add(appt1pm());
            appts[1].ExternalData.ScreeningMessageSent = true;
            appts[1].ExternalData.ScreeningMessageResponse = true;

            msgs.Add(new SmsMessage("+61411012345", "I'm here"));

            // run it
            engine.ProcessIncomingMessages(appts, msgs);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("The robot processing this message is stupid, and didn't understand your response. Please just say \"arrived\", or phone {num} for help", OutputMsgs[0].message);
            Assert.AreEqual(0, StorageOps.Count);
        }


        [TestMethod]
        public void testUnexpected()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 58, 0);
            reset();
            List<PmsAppointment> appts = new List<PmsAppointment>();
            List<SmsMessage> msgs = new List<SmsMessage>();
            // set it up:
            appts.Add(appt10am());
            appts.Add(appt1pm());
            appts[1].ExternalData.ScreeningMessageSent = true;
            appts[1].ExternalData.ScreeningMessageResponse = true;
            appts[1].ArrivalStatus = AppointmentStatus.Fulfilled;

            msgs.Add(new SmsMessage("+61411012345", "Arrived"));

            // run it
            engine.ProcessIncomingMessages(appts, msgs);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Patient Test Patient #2 has an appointment with Dr Adam Ant at 01:00 PM on 1-Jan, but this robot is not expecting a message right now", OutputMsgs[0].message);
            Assert.AreEqual(0, StorageOps.Count);
        }

        [TestMethod]
        public void testVideoJoined()
        {
            MessagingEngine engine = makeEngine();
            var fhirServerUpdater = new MessageLogicFhirUpdaterHandler(this);
            engine.AppointmentUpdater = fhirServerUpdater;
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 58, 0);
            reset();
            List<PmsAppointment> appts = new List<PmsAppointment>();
            List<SmsMessage> msgs = new List<SmsMessage>();
            // set it up:
            appts.Add(appt10am());
            appts.Add(appt1pm());
            appts[1].ExternalData.ScreeningMessageSent = true;
            appts[1].ExternalData.ScreeningMessageResponse = true;
            appts[1].IsVideoConsultation = true;
            appts[1].ExternalData.VideoInviteSent = true;

            msgs.Add(new SmsMessage("+61411012345", "Joined"));

            // run it
            engine.ProcessIncomingMessages(appts, msgs);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("Thank you. The Doctor will join you as soon as possible", OutputMsgs[0].message);
            Assert.AreEqual(1, StorageOps.Count);
            Assert.AreEqual("1002", StorageOps[0].Appointment.AppointmentFhirID);
            Assert.IsTrue(StorageOps[0].Appointment.ArrivalStatus == AppointmentStatus.Arrived);
            Assert.IsTrue(StorageOps[0].Appointment.ExternalData.ArrivalStatus == AppointmentStatus.Arrived);
            Assert.AreEqual("save-status", FhirUpdateOps[0].type);
            Assert.AreEqual(AppointmentStatus.Arrived, FhirUpdateOps[0].status);
        }

        [TestMethod]
        public void testVideoJoinedDuh()
        {
            MessagingEngine engine = makeEngine();
            engine.TimeNow = new DateTime(2021, 1, 1, 12, 58, 0);
            reset();
            List<PmsAppointment> appts = new List<PmsAppointment>();
            List<SmsMessage> msgs = new List<SmsMessage>();
            // set it up:
            appts.Add(appt10am());
            appts.Add(appt1pm());
            appts[1].ExternalData.ScreeningMessageSent = true;
            appts[1].ExternalData.ScreeningMessageResponse = true;
            appts[1].IsVideoConsultation = true;
            appts[1].ExternalData.VideoInviteSent = true;

            msgs.Add(new SmsMessage("+61411012345", "ok ready"));

            // run it
            engine.ProcessIncomingMessages(appts, msgs);
            // inspect outputs:
            Assert.AreEqual(1, OutputMsgs.Count);
            Assert.AreEqual("+61411012345", OutputMsgs[0].phone);
            Assert.AreEqual("The robot processing this message is stupid, and didn't understand your response. Please just say \"joined\" when you have joined the video call", OutputMsgs[0].message);
            Assert.AreEqual(0, StorageOps.Count);
        }

        // test case generation ----------------------------------------------------------

        private PmsAppointment tomorrowsAppointment()
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient #1";
            app.PatientMobilePhone = "+0411012345";
            app.PractitionerName = "Dr Adam Ant";
            app.PractitionerFhirID = "p123";
            app.AppointmentFhirID = "1234";
            app.ArrivalStatus = AppointmentStatus.Booked;
            app.AppointmentStartTime = new DateTime(2021, 1, 2, 9, 15, 0);
            return app;
        }

        private PmsAppointment appt10am()
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient #1";
            app.PatientMobilePhone = "+0411012346";
            app.PractitionerName = "Dr Adam Ant";
            app.PractitionerFhirID = "p123";
            app.AppointmentFhirID = "1000";
            app.ArrivalStatus = AppointmentStatus.Booked;
            app.AppointmentStartTime = new DateTime(2021, 1, 1, 10, 0, 0);
            return app;
        }

        private PmsAppointment appt1pm()
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient #2";
            app.PatientMobilePhone = "0411012345";
            app.PractitionerName = "Dr Adam Ant";
            app.PractitionerFhirID = "p123";
            app.AppointmentFhirID = "1002";
            app.ArrivalStatus = AppointmentStatus.Booked;
            app.AppointmentStartTime = new DateTime(2021, 1, 1, 13, 0, 0);
            return app;
        }

        private PmsAppointment appt2pm()
        {
            PmsAppointment app = new PmsAppointment();
            app.PatientFhirID = Guid.NewGuid().ToString();
            app.PatientName = "Test Patient #3";
            app.PatientMobilePhone = "+0411012345";
            app.PractitionerName = "Dr Adam Ant";
            app.PractitionerFhirID = "p123";
            app.AppointmentFhirID = "1002";
            app.ArrivalStatus = AppointmentStatus.Booked;
            app.AppointmentStartTime = new DateTime(2021, 1, 1, 14, 0, 0);
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
            engine.TemplateProcessor = new TemplateProcessor();
            engine.TemplateProcessor.Initialise(testSettings());
            engine.VideoManager = new MessageLogicTesterVideoHandler(this);
            engine.RoomMappings = new System.Collections.ObjectModel.ObservableCollection<DoctorRoomLabelMapping>();
            engine.Logger = new MessageLogicTesterLogger();
            loadRoomMappings(engine.RoomMappings);
            loadTestTemplates(engine.TemplateProcessor);
            return engine;
        }

        private void loadRoomMappings(ObservableCollection<DoctorRoomLabelMapping> roomMappings)
        {
            roomMappings.Add(new DoctorRoomLabelMapping("p123", "Please go to room 7."));
        }

        private void loadTestTemplates(TemplateProcessor tp)
        {
            tp.Templates = new System.Collections.ObjectModel.ObservableCollection<MessageTemplate>();
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_REGISTRATION, "Patient {{Patient.name}} has an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}. 3 hours prior to the appointment, you will be sent a COVID-19 screening check to decide whether you should do a video consultation rather than seeing the doctor in person"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_SCREENING, "Please consult the web page http://www.rcpa.org.au/xxx to determine whether you are eligible to meet with the doctor by phone/video. If you are, respond to this message with YES otherwise respond with NO"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_SCREENING_YES, "Thank you. Do not come to the doctor's clinic. You will get an SMS message containing the URL for your video meeting a few minutes before your appointment. You can join from any computer or smartphone"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_SCREENING_NO, "Thank you. When you arrive at the clinic, stay in your car (or outside) and reply \"arrived\" to this message"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_VIDEO_INVITE, "Please start your video call at {{url}}. When you have started it, reply to this message with the word \"joined\""));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_APPT_READY, "The doctor is ready to see you now. {{room}}"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_UNKNOWN_PH, "This phone number is not associated with an appointment to see the doctor today. Please phone {num} for help"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_SCREENING, "The robot processing this message is stupid, and didn't understand your response. Please answer yes or no, or phone {num} for help"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_ARRIVED_THX, "Thanks for letting us know that you're here. We'll let you know as soon as the doctor is ready for you"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_ARRIVING, "The robot processing this message is stupid, and didn't understand your response. Please just say \"arrived\", or phone {num} for help"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_UNEXPECTED, "Patient {{Patient.name}} has an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}, but this robot is not expecting a message right now"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_VIDEO_THX, "Thank you. The Doctor will join you as soon as possible"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_VIDEO, "The robot processing this message is stupid, and didn't understand your response. Please just say \"joined\" when you have joined the video call"));
        }

        private void reset()
        {
            OutputMsgs.Clear();
            StorageOps.Clear();
        }


        private class MessageLogicTesterLogger : ILoggingService
        {
            public void Log(int level, string msg)
            {
                Debug.WriteLine(msg);
            }
        }

        private class MessageLogicTesterVideoHandler : IVideoConferenceManager
        {
            private MessageEngineTester owner;

            public MessageLogicTesterVideoHandler(MessageEngineTester messageLogicTester)
            {
                this.owner = messageLogicTester;
            }
            public void Initialize(Settings settings)
            {
            }

            public bool canKnowIfJoined()
            {
                return false;
            }

            public string getConferenceUrl(string appointmentId)
            {
                return "https://meet.jit.si/:guid:-" + appointmentId;
            }

            public bool hasSomeoneJoined(string appointmentId)
            {
                return false;
            }
        }

        private class MessageLogicFhirUpdaterHandler : IFhirAppointmentUpdater
        {
            private MessageEngineTester owner;

            public MessageLogicFhirUpdaterHandler(MessageEngineTester messageLogicTester)
            {
                this.owner = messageLogicTester;
            }


            public void SaveAppointmentAsVideoMeeting(PmsAppointment appt, string videoLinkComment)
            {
                // Get the Appointment based on the appointment having an ID
                // Hl7.Fhir.Model.Appointment fhirAppt = server.Get(appt.FhirAppointmentID);
                // fhirAppt.AppointmentType = new CodeableConcept("http://hl7.org/au/fhir/CodeSystem/AppointmentType", "teleconsultation");
                // fhirAppt.Comment = String.IsNullOrEmpty(fhirAppt.Comment) ?
                //    videoLinkComment
                //    : fhirAppt.Comment +
                //    Environment.NewLine + Environment.NewLine +
                //    videoLinkComment;
                owner.FhirUpdateOps.Add(new FhirUpdateOp("save-video-meeting", appt.AppointmentFhirID) { comment = videoLinkComment });
            }

            public void SaveAppointmentStatusValue(PmsAppointment appt)
            {
                // Get the Appointment based on the appointment having an ID 
                // and update the status value
                // Hl7.Fhir.Model.Appointment fhirAppt = server.Get(appt.FhirAppointmentID);
                // fhirAppt.Status = appt.Status;
                // server.Update(fhirAppt);
                owner.FhirUpdateOps.Add(new FhirUpdateOp("save-status", appt.AppointmentFhirID) { status = appt.ArrivalStatus });
            }
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

            public Task LoadAppointmentStatus(DateTime date, PmsAppointment appt)
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<DoctorRoomLabelMapping>> LoadRoomMappings()
            {
                throw new NotImplementedException();
            }

            public Task<Settings> LoadSettings()
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<MessageTemplate>> LoadTemplates()
            {
                throw new NotImplementedException();
            }

            public Task<IEnumerable<SmsMessage>> LoadUnprocessableMessages(string date)
            {
                throw new NotImplementedException();
            }

            public Task SaveAppointmentStatus(DateTime date, PmsAppointment appt)
            {
                owner.StorageOps.Add(new StorageOp("save-appt", appt));
                return null;
            }

            public Task SaveRoomMappings(IEnumerable<DoctorRoomLabelMapping> mappings)
            {
                throw new NotImplementedException();
            }

            public Task SaveSettings(Settings settings)
            {
                throw new NotImplementedException();
            }

            public Task SaveTemplates(IEnumerable<MessageTemplate> templates)
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

            public StorageOp(string type, PmsAppointment Appointment)
            {
                this.type = type;
                this.Appointment = Appointment;
            }

            public PmsAppointment Appointment { get; set; }
            public string type { get; set; }

        }

        private class FhirUpdateOp
        {
            public FhirUpdateOp(string type, string FhirAppointmentId)
            {
                this.type = type;
                this.FhirAppointmentId = FhirAppointmentId;
            }

            public string FhirAppointmentId { get; set; }
            public string type { get; set; }
            public AppointmentStatus status;
            public string comment;
        }
    }

}
