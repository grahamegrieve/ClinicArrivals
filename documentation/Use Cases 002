# Clinic Arrivals – Use Cases

#### Version list
* 001 – initial version
* 002 – added post-appointment message, telemedicine path, user roles


#### User roles

1. Patient
2. Front desk
3. Practitioner
4. Practice IT support team
5. Practice Management Software

## UseCase01. A GP sign-ups to use the app

#### Users
* Practitioner
* Practice IT support
#### Goal/Result
Workflow started: Messages are sending and processing, Front Desk sees lists of approved appointments, arrived patients, invited patients, tele-calls in progress (?).
#### Brief description
GP creates an account and configures the app
#### Condition
GP software supports FHIR v4
#### Steps
1. Practitioner goes to application page and creates an account
2. Practitioner configures Practice name, Messages text
3. Practice IT support team configures integration

#### Alternative1 - no integration
Theoretically we could allow to register appointments manually. 
Possibly will not support this case.

#### Alternative2 (addition) - app for the patient
(this is a proposed functionality, a decision to implement it was not made)

If the app for patients is supported,  so Practice has to encourage patients to download the app from AppStore/GooglePlay. Publish that on the site, tell it by every appointment made by phone.

## Use Case 02. A patient arranges an appointment and agrees on some visit type (in person or telecons)
#### Users
* Patient
* FrontDesk
* Practice Management Software

#### Goal/Result
Patient: has a confirmed appointment with an agreed visit type
FrontDesk: sees patient in one of the scheduled appointment lists
#### Brief description
Patient makes an appointment. App figures out the new appointment appeared in PMS and sends a questionnaire. Patient decides what type of visit is appropriate and sends an answer. App sets an appropriate status to the appointment so it can be seen by Frontdesk in the appropriate list.
#### Condition
no
#### Steps
1. Patient arranges an appointment using Patient Portal in PMS or by phone.
2. Patient tells his phone number. If patient is going to come with a carer, it should be carer's phone number. 
3. PMS stores the appointment.
4. App figures out that a new appointment appeared in PMS
5. App sends a questionnaire (or a link to) to the patient phone number by SMS.
6. App shows the Appointment in (? WAITING FOR PATIENT ANSWER) list
7. If Patient answers (YES) App shows the Appointment in the (? TELECONS SCHEDULED) list.
8. If Patient answers (NO) App shows the Appointment in the (? IN PERSON SCHEDULED) list.
10. If Patient answers something else, App shows the Appointment in the (?ERROR ANSWER) list with some comment
9. If Patient doesn't answer for XX minutes App shows the Appointment in the (?ERROR ANSWER) list with some comment

#### Alternative1 - not the earliest appointment for the day
If the appointment is not the earliest for the day, we ignore this appointment (is that true?).

#### Alternative2 - if the appointment is not for the same day
In this case 

1) Additional Step 4a:
App sends to the patient a preliminary message (SMS)
"Thank you making an appointment to see Dr X. [X] hours before the appointment, we will send you an SMS asking with you meet the criteria documented at http://www.rcpa.org.au/xxx, to decide whether you will talk to the doctor by telephone video, or physically come to the clinic. Please respond to this message to confirm you have seen it or your appointment will be cancelled."

2) Step 5 will be done at X hours (e.g. 2-3) before the scheduled appointment time


 ## Use Case 03. Teleconsultation 
#### Users
* Patient
* Physician
* FrontDesk
* Practice Management Software

#### Goal/Result
Patient: is consulted distantly
Physician: has consulted distantly, has information in patient's EMR
FrontDesk: sees tele consultation status at any time
#### Brief description
App registers a meeting with jitsi. Patient comes at scheduled time, doctor comes when ready.
#### Condition
Appointment Status is (? TELECONS SCHEDULED)
#### Trigger
Just after receiving answer from patient.
#### Steps
1. App registers a meeting with Jitsi --> [id]
2. App adds the URL for the meeting to the notes for the appointment 
3. App sends the URL to the patient with a note to join the meeting:
You will meet with the Doctor by phone/video. At [2:15pm], open the URL http://meet.jit.si/[id]. (Note: if you join from a mobile phone, you will need to install the jitsi app first).
4. At appropriate time: Patient joins the room
5. App marks Patient as arrived in the PMS 
6. When the doctor is ready to see the patient, they copy the URL out of the appointment notes and paste into their browser
#### Alternative1 - patient does not come
We don't process the situation, do we? FrontDesk will do it as usual.
#### Alternative2 - appointment cancelled before it's start
App cancels the meeting with jitsi

## Use Case 04. Consultation in person 
#### Users
* Patient
* Physician
* FrontDesk
* Practice Management Software

#### Goal/Result
Patient: is consulted
Physician: has consulted
FrontDesk: sees appoinment status at any time
#### Brief description
Patient arrives and sends "ARRIVED" SMS. When doctor is ready, Frontdesk invites the patient.
#### Condition
Appointment Status is (? IN PERSON SCHEDULED)
#### Trigger
Just after receiving answer from patient.
#### Steps
1. Send patient SMS like this:
"Due to the COVID-19 pandemic, the clinic no longer has an open patient waiting room. When you arrive at the clinic, stay in your car and respond to this SMS with the word "arrived". The wait in your car until you are invited in. If you do not arrive by car, then wait [instructions]"
2. Patient arrives. Additionally, they see a sign on the door something like:
"Due to the COVID-19 Pandemic, this clinic has closed it's waiting room. Please wait in your car, and SMS "arrived" to [phone number]
3. Patient sends "ARRIVED" by SMS
4. App shows the Appointment in (ARRIVED) list.
5. When doctor is ready, Frondesk or Doctor, in the PMS, mark the consult as 'in-process'.
6. The Application will send the patient this message:
"The doctor is ready for you now. Please come in to room ##"
The room number will be looked up for the assigned label for the doctor in the application. If there's no room number, it will just say "Please come in now".
#### Alternative1 - no answer "ARRIVED" from the patient
We don't process the situation, do we? FrontDesk will do it as usual.
#### Alternative2 - answer "ARRIVED" from inappropriate phone number (that doesn't match to any appointment)
1. the incoming mobile number does not match to a known patient: reply with "This is not a recognised phone number. Please respond with details about the patient who the appointment is for (name, dob, medicare number) ?
2. if the patient is found, remember the link
3. if the patient with appointment is still not found, App puts the appointment to the Error list for FrontDesk
#### Alternative3 - appointment cancelled before it's start
App just cancels the appointment, and removes from the lists.


## Alternative steps if we have a downloadable app for the patient
### UseCase01. A GP sign-ups to use the app
Steps
1. Patient arranges an appointment using Patient Portal in PMS or by phone.
2. Patient downloads app.
3. PMS stores the appointment.
4. App figures out that a new appointment appeared in PMS
5. App shows the questionnaire to the patient and asks for an answer.
6. App shows the Appointment in (? WAITING FOR PATIENT ANSWER) list.
7. If Patient answers (YES) App shows the Appointment in the (? TELECONS SCHEDULED) list.
8. If Patient answers (NO) App shows the Appointment in the (? IN PERSON SCHEDULED) list.
9. If Patient doesn't answer for XX minutes App shows the Appointment in the (?ERROR ANSWER) list with some comment

### Use Case 02. A patient arranges an appointment and agrees on some visit type (in person or telemed)
Steps

1. Patient arranges an appointment using Patient Portal in PMS or by phone.
2. Patient downloads App. 
3. PMS stores the appointment.
4. App figures out that a new appointment appeared in PMS
5. App shows a questionnaire to the patient and asks for an answer.
6. App shows the Appointment in (? WAITING FOR PATIENT ANSWER) list
7. If Patient answers (YES) App shows the Appointment in the (? TELECONS SCHEDULED) list.
8. If Patient answers (NO) App shows the Appointment in the (? IN PERSON SCHEDULED) list.
9. If Patient doesn't answer for XX minutes App shows the Appointment in the (?ERROR ANSWER) list with some comment

## Use Case 03. Teleconsultation 
Steps
1. App registers a meeting with Jitsi --> [id]
2. App adds the URL for the meeting to the notes for the appointment 
3. App shows  to the patient a note to join the meeting:
You will meet with the Doctor by phone/video. At [2:15pm], just be here
4. At appropriate time: App starts jitsi
5. Patient joins the room
5. App marks Patient as arrived in the PMS 
6. When the doctor is ready to see the patient, they copy the URL out of the appointment notes and paste into their browser


### Use Case 04. Consultation in person 
Steps
1. App shows the patient text like this:
"Due to the COVID-19 pandemic, the clinic no longer has an open patient waiting room. When you arrive at the clinic, stay in your car and respond to this SMS with the word "arrived". The wait in your car until you are invited in. If you do not arrive by car, then wait [instructions]"
2. Patient arrives. Additionally, they see a sign on the door something like:
"Due to the COVID-19 Pandemic, this clinic has closed it's waiting room. Please wait in your car, and SMS "arrived" to [phone number]
3. Patient hits "ARRIVED" button
4. App shows the Appointment in (ARRIVED) list.
5. When doctor is ready, Frondesk or Doctor, in the PMS, mark the consult as 'in-process'.
6. The Application shows the the patient this message:
"The doctor is ready for you now. Please come in to room ##"
The room number will be looked up for the assigned label for the doctor in the application. If there's no room number, it will just say "Please come in now".
