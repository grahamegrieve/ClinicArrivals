# Templates

All SMS messages that the application sends are based on a template.

Templates are managed using the `Message Templates` tab in the 
Application.

## Template Syntax

The templates use [Liquid](https://shopify.github.io/liquid/) as the language. 

The template looks like this:

    This is a message for {{Patient.name}}.
    
The template processor will replace all the variables that are surrounded 
with ```{{}}``` with their actual value, to get this:

    This is a message for John Smith.

The langauge is very capable - see [Liquid](https://shopify.github.io/liquid/) for further details.

## Template Variables

The following variable names are supported by the application:

* ```Patient.name```: The name of the patient
* ```Practitioner.name```: The name of the doctor the patient is seeing
* ```Appointment.start```: The start date/time of the appointment
* ```Appointment.start.date```: The start date of the appointment (dd-mmm)
* ```Appointment.start.time```: The start time of the appointment (hh:nn AM/PM)

Included for internal debugging purposes, not so useful for patient messages:

* ```Patient.telecom.mobile```: The phone number for the patient 
* ```Appointment.status```: The status of the appointment
* ```Practitioner.id```: Internal ID of the pracititioner
* ```Patient.id```: Internal ID of the patient 
* ```Appointment.id```: Internal ID of the appointment

In addition, there are some event specific variables. The following events 
are defined:


### Registration

This message is sent when an appointment is due in 1-2 days (but not today).

A typical example:

    Patient {{Patient.name}} has an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}. 3 hours prior to the appointment, you will be sent an SMS referring to a COVID-19 screening check to decide whether you should talk to the doctor by phone/video rather than seeing the doctor in person. Please do not respond to this message

If video messaging is not in use, the message should not talk about the choice, as this would not be applicable.

### Cancellation

This message is sent when the appointment is cancelled. Note: you can't cancel via SMS, and this message isn't currently used/supported

A typical example:

    The appointment for Patient {{Patient.name}} with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}} has been cancelled

### UnknownPhone

The reply when a message is received from a phone number not associated with any appointment. This is typically something that happens in response 
to the welcome on the clinic door, when a patient's carer calls, or there is no phone number associated with the patient. The phone number 
should be the reception desk, and they should enter the mobile phone number against the patient, which will start the screening workflow.

A typical example:

    The phone number you sent the message from is not associated with an appointment to see the doctor today. Please phone [clinic number] for help

### TooManyAppointments

This message is sent when a message comes from a phone when patients associated with the phone have more than 3 or more appointments on the one day, 
or sometimes if there's 2 appointments in the day but the engine can't work out where it is in the workflow.

A typical example:

    The robot processing this message couldn't figure out which appointment of multiple for this day that this message was about. Please phone [clinic number] for help

### Unexpected

This message is sent when a message arrives from a mobile phone that is associated with an appointment, but the program can't figure out where 
it is in a messaging workflow - typically, this means that the patient sent an unexpected message

A typical example:

    Patient {{Patient.name}} does have an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}, but this robot is not expecting a message right now. Please phone [clinic number] if you need help

### ConsiderTeleHealth

This message is sent 2-3 hours in advance of the consultation to find out whether a video consulation is appropriate, or whether the patient 
should come in. The exact wording of the message adapts to condition. Whatever the question is, the answer is "yes" for a video consultation,
and "no" for a physical in person consultation. 

A typical example:

    Patient {{Patient.name}} does have an appointment with {{Practitioner.name}} at {{Appointment.start.time}} on {{Appointment.start.date}}. If you have symptoms of Covid-19, or exposure to a known case, you MUST choose to talk to the doctor by telephone/video, otherwise, you should choose to this unless you really need to come to the clinic. Respond to this message with YES to choose to telephone/video consultation, otherwise respond with NO

If the appointment is already marked as telehealth consultation, the video welcome message will be sent instead (see below)

### ScreeningYesToVideo

This message is sent to the patient after they send "yes" in response to the previous question.

A typical example:

    Thank you. Do not come to the doctor's clinic. Your doctor will call you for your appointment. When you are ready for your appointment, reply to this message with the word 'waiting'. If the doctor wants to see you by video, they will ask you to follow a link you will be sent before the appointment. You can join from any computer or smartphone. For instructions, see https://bit.ly/2vFGl2c

### ScreeningNoToVideo

This message is sent to the patient after they send "no" in response to the previous question.

A typical example:

    Thank you. When you arrive at the clinic, stay in your car (or outside the clinic) and reply "arrived" to this message

### ScreeningDontUnderstand

This message is sent to the patient if they reply something other than "yes" or "no" to the previous question.

A typical example:

    The robot processing this message didn't understand your response. Please answer yes or no, or phone [clinic number] for help

### VideoInvite

If the consultation has been marked as a video consultation by either the previous exchange or by the reception staff, then this work flow applies. 
This message is sent 10 min or so before the appointment is due so the patient can get ready. 

A typical example:

    Patient {{Patient.name}} does have an appointment with {{Practitioner.name}} at {{Appointment.start.time}} is happening soon. The doctor will ring on this number. If the doctor wants to see you, the link is {{url}}. Reply "ready" when you are ready

Note: if the patient follows the link and sets the video call up, we'll automatically assume they are ready 

**Event specific variable**:

* ```url```: The url the patient should click on to join the video call

### VideoThanks

Sent to the patient in response to them sending "joined".

A typical example:

    Thank you. The Doctor will call you as soon as possible

### VideoDontUnderstand

This mesage is sent in response to the patient sending something else instead of "joined".

A typical example:

    The robot processing this message didn't understand your response. Please just say "ready" when you are ready for the call

### ArrivedThanks

The following messages apply when the patient is actually turning up.

This is sent in response a recognised phone number sending "Arrived".

A typical example:

    Thanks for letting us know that you're here. We'll let you know as soon as the doctor is ready for you

### DoctorReady

This is sent to the patient when the reception staff mark the consultation has 'in progress' in the PMS.
The room comes from the configuration for the doctor in the application.

A typical example:

    The doctor is ready to see you now. {{room}}

**Event specific variable**:

* ```room```: Specific instructions for how to go to the doctors room. This may be empty if nothing is configured for the doctor


### ArrivingDontUnderstand

This is sent in response when we expected "arrived" but got something else.

A typical example:

    The robot processing this message didn't understand your response. Please just say "arrived", or phone [clinic number] for help

