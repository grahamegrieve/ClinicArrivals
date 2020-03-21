# Templates

Each SMS message that is sent is based created using a template.
Tempaltes are maanged using the Message Templates tab in the 
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

In addition, there are some event specific variables:

### Post-Registration Message

This message is sent on a day or 2 prior to the appointment. 

It has no event specific variables

### Screening Message

This message is sent 2-3 hours before the appointment to ask the patient 
to perform screening for appropriateness for a video consultation. 

It has no event specific variables

### Video Invitation

This message is sent 10 minutes prior to the appointment to invite the patient
to join the video conference. 

It has an event specific variable:

* ```url```: The url the patient should click on to join the video call

