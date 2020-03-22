# Templates

Each SMS message that is sent is based created using a template.
Templates are maanged using the Message Templates tab in the 
Application.

## Template Syntax

The templates use [Liquid]{https://shopify.github.io/liquid/} as the language. 

The concept is very simple. The template looks like this:

    This is a message for {{Patient.name}}.
    
The template processor will replace all the variables that are surrounded 
with ```{{}}``` with their actual value, to get this:

    This is a message for John Smith.

The langauge is very capable - see [Liquid]{https://shopify.github.io/liquid/} for further details.

## Template Variables

The following variable names are supported by the application:

* ```Patient.name```: The name of the patient
* ```Practitioner.name```: The name of the doctor the patient is seeing
* ```Appointment.start```: The start date/time of the appointment
* ```Appointment.start.date```: The start date of the appointment (dd-mmm)
* ```Appointment.start.time```: The start time of the appointment (hh:nn AM/PM)

Included for internal debugging purposes, not so useful for patient messages

* ```Patient.telecom.mobile```: The phone number for the patient 
* ```Appointment.arrival```: The arrival time of the patient
* ```Appointment.status```: The status of the appointment
* ```Practitioner.id```: 
* ```Patient.id```: 
* ```Appointment.id```: 

In addition, there are some event specific variables. THe following events 
are defined. 

        public const string MSG_VIDEO_INVITE = "VideoInvite";
        public const string MSG_UNEXPECTED = "Unexpected";
        public const string MSG_VIDEO_THX = "VideoThanks";
        public const string MSG_DONT_UNDERSTAND_VIDEO = "VideoDontUnderstand";
        public const string MSG_TOO_MANY_APPOINTMENTS = "TooManyAppointments";


        public const string MSG_REGISTRATION = "Registration";
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_REGISTRATION, "Patient {{Patient.name}} has an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}. 3 hours prior to the appointment, you will be sent a COVID-19 screening check to decide whether you should do a video consultation rather than seeing the doctor in person"));
        public const string MSG_CANCELLATION = "Cancellation";
        public const string MSG_SCREENING = "ConsiderTeleHealth";
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_SCREENING, "Please consult the web page http://www.rcpa.org.au/xxx to determine whether you are eligible to meet with the doctor by phone/video. If you are, respond to this message with YES otherwise respond with NO"));
        public const string MSG_SCREENING_YES = "ScreeningYesToVideo";
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_SCREENING_YES, "Thank you. Do not come to the doctor's clinic. You will get an SMS message containing the URL for your video meeting a few minutes before your appointment. You can join from any computer or smartphone"));
        public const string MSG_SCREENING_NO = "ScreeningNoToVideo";
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_SCREENING_NO, "Thank you. When you arrive at the clinic, stay in your car (or outside) and reply \"arrived\" to this message"));
        public const string MSG_DONT_UNDERSTAND_SCREENING = "ScreeningDontUnderstand";
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_SCREENING, "The robot processing this message is stupid, and didn't understand your response. Please answer yes or no, or phone {num} for help"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_VIDEO_INVITE, "Please start your video call at {{url}}. When you have started it, reply to this message with the word \"joined\""));
        public const string MSG_APPT_READY = "DoctorReady";
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_APPT_READY, "The doctor is ready to see you now. {{room}}"));
        public const string MSG_UNKNOWN_PH = "UnknownPhone";
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_UNKNOWN_PH, "This phone number is not associated with an appointment to see the doctor today. Please phone {num} for help"));
        public const string MSG_ARRIVED_THX = "ArrivedThanks";
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_ARRIVED_THX, "Thanks for letting us know that you're here. We'll let you know as soon as the doctor is ready for you"));
        public const string MSG_DONT_UNDERSTAND_ARRIVING = "ArrivingDontUnderstand";
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_ARRIVING, "The robot processing this message is stupid, and didn't understand your response. Please just say \"arrived\", or phone {num} for help"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_UNEXPECTED, "Patient {{Patient.name}} has an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}, but this robot is not expecting a message right now"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_VIDEO_THX, "Thank you. The Doctor will join you as soon as possible"));
            tp.Templates.Add(new MessageTemplate(MessageTemplate.MSG_DONT_UNDERSTAND_VIDEO, "The robot processing this message is stupid, and didn't understand your response. Please just say \"joined\" when you have joined the video call"));

### Video Invitation

This message is sent 10 minutes prior to the appointment to invite the patient
to join the video conference. 

It has an event specific variable:

* ```url```: The url the patient should click on to join the video call

### Appointment Ready

This message is sent once the doctor is ready for the patient to come into the room. 

It has an event specific variable:

* ```room```: Specific instructions for how to go to the doctors room. This may be empty if nothing is configured for the doctor

