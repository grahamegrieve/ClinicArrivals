# Documentation 

## Conceptual Design

The application is appointment based. 

### Pre-arrival

if a mobile phone is registered for the patient

2 hours before appointment, it sends and SMS to the person saying something like
"Due to the COVID-19 pandemic, the clinic no longer has an open patient waiting room.
When you arrive at the clinic, stay in your car and respond to this SMS with the word 
"arrived". The wait in your car until you are invited in. If you do not arrive by car,
then wait [instructions]"

 - note: for non care arrivals (taxis etc, the clinic needs to figure out least worst 
   instructions whether it's the waiting room, or 
   
Clinic has a sign on the door something like:

"Due to the COVID-19 Pandemic, this clinic has closed it's waiting room. Please wait
in your car, and SMS "arrived" to 

#### Error conditions

If the patient replies with anything other than "arrived", then....?

### Arrival

When the patient replies with "arrived" then 
If:
* the incoming mobile number does not match to a known patient: reply with "This is not a recognised phone number. Please respond with details about the patient who the appointment is for (name, dob, medicare number) ?
  * if the patient is found, remember the link? 
  * if the patient is still not found, now what?
* the mobile number matches a patient without an appointment: ?
* the mobile number matches an appointment: ?

### Appointment

Once the clinic manager assigns a room and invites the patient (may need to do this 
a little earlier since patient will need to walk further), system sends email with
text like:

"The doctor is ready for you now. Please come in to room ##"

## Carers

Note: Many patients are brought in by someone else, and so the phone may be 
associated with a carer not the patient (e.g. parent, partner or son/daughter). 

TODO: how much support does the PMS have for tracking associated mobile phone numbers? 
* include a database for the application so it can remember links made ?

## PMS Interface

The system works with multiple PMS software. these details are abstracted behind
the PMS interface. The interface has the following features:
