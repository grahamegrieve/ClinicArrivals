# Workflow Description

The application is appointment-based. 

## Post registration

As soon as an appointment is registered, the system sends the patient an SMS:

"Thank you for making an appointment to see Dr X. [X] hours before the appointment, we will send you an SMS asking with you meet the criteria documented at http://www.rcpa.org.au/xxx, to decide whether you will talk to the doctor by telephone video, or physically come to the clinic. Please respond to this message to confirm you have seen it (or your appointment will be cancelled)"

This message is only sent for appointments on the next day or the day after. It is not sent for appointments that are made on the same day.

### Error conditions

If the patient replies with anything, then an error message is sent back to the patient advising them to talk to reception if anything is needed.

## Pre-appointment 

X hours (e.g. 2-3) before the appointment, send a message to the patient:

"Please consult the web page http://www.rcpa.org.au/xxx to determine whether you are eligible to meet with the doctor by phone/video. If you are, respond to this message with YES otherwise respond with NO"

Notes:
* the time is configurable in the application 
* appointments that are made within the X hours time window - right up to now - will still cause this message to be sent. 

## Teleconsulation pathway

If the patient is eligible for a teleconsultation (reply matches 'yes') then:

* ClinicArrivals app registers a meeting with OpenVidu --> [id]
* the URL for the meeting is added to the notes for the appointment
* the URL is sent to the patient with a note to join the meeting:

"You will meet with the Doctor by phone/video. At [2:15pm], open the URL http://meet.jit.si/[id]. (Note: if you join from a mobile phone, you will need to install the jitsi app first)."

When the patient joins the room, they will be marked as arrived in the PMS. When the doctor is ready to see the patient, they copy the URL out of the appointment notes and paste into their browser. 

## Physical meeting pathway

If the patient is not eligible for a teleconsultation (reply matches 'no') then:

Send patient SMS like this:

"Due to the COVID-19 pandemic, the clinic no longer has an open patient waiting room. When you arrive at the clinic, stay in your car and respond to this SMS with the word "arrived". The wait in your car until you are invited in. If you do not arrive by car, then wait [instructions]"

 - note: for non care arrivals (taxis etc), the clinic needs to figure out least-worst instructions whether it's the waiting room, or
   
Clinic has a sign on the door something like:

"Due to the COVID-19 Pandemic, this clinic has closed it's waiting room. Please wait
in your car, and SMS "arrived" to [phone number]"

### Error conditions

If the patient replies with anything other than "arrived", then....?

## Arrival

When the patient replies with "arrived" then:

If:
* the incoming mobile number does not match to a known patient: reply with
  - "This is not a recognised phone number. Please respond with details about the patient who the appointment is for (name, dob, medicare number) ?"
  * if the patient is found, remember the link? 
  * if the patient is still not found, now what?
* the mobile number matches a patient without an appointment: ?
* the mobile number matches an appointment: ?

## Appointment

In the PMS, mark the consult as 'in-process'. The Application will send the patient this message: 

"The doctor is ready for you now. Please come in to room ##"

The room number will be looked up for the assigned label for the doctor in the application. If there's no room number, it will just say "Please come in now".

## Carers

Note: Many patients are brought in by someone else, and so the phone may be associated with a carer not the patient (e.g. parent, partner or son/daughter).

TODO: how much support does the PMS have for tracking associated mobile phone numbers? 
* include a database for the application so it can remember links made?

## PMS Interface

The interface to the PMS is a FHIR interface (R4). 
