# User Guide

The ClinicArrivals Program is mainly intended to be ignored while running - it will look after itself. Users interact with the program for four different reasons:

* checking on the status of the program (ongoing monitoring)
* managing the room arrangements and message wording (ongoing configuration)
* performing initial configuration (only while setting up)
* testing the behavior of the application (only when workflows are being changed)

The ClinicArrivals application is tricky to conceptualise and manage because 
it's a glue application between 3 different systems:
* A Practice Management System (PMS) - this manages the clinic and maintains the appointments 
* Twilio - an SMS sending/receiving system
* OpenVidu - an open source WebRTC based video-conferencing system

It watches and receives input from these systems and routes it to the other systems. 

## Tabs

The application has the following tabs: 

* **Status**: Summary of the overall health status of the program - check for any problems at a glance 
* **Appointments**: Detailed display of expected appointments for the day, and currently arrived patients (to help with troubleshooting)
* **Unknown Incoming Messages**: List of SMS messages that have arrived that weren't understood (see note below)
* **Sms Simulator**: Allows testing the application by simulating sending SMS messages to it (and seeing what has been sent)
* **Pms Simulator**: Allows testing the application by creating appointments
* **Room Mappings**: List of instructions for finding the room for a particular doctor
* **Message Templates**: Controls the actual text of the messages that get sent out to patients
* **Settings**: Application configuration
* **About**: Information about the program

## Checking on the status of the program

Because the application is glue between three other systems, issues with any of these systems or connectivity to them can interrupt the normal running of the program.

You can check on the program by watching the counts of the appointments with different status, and how many SMS messages have been sent and received, and also by how long it is since any numbers changed.

In addition, for ease of review, the application lists which appointments are still expected for the day, and which have patients who have arrived (either in the carpark, or on video). Note that this list only includes patients that have a mobile phone associated with the appointment; other appointments are ignored and must be managed another way.

Patients are asked to respond to SMS messages that are sent to them with one word answers. Some patients will respond out of time, or with messages that are not understood by the program. These messages are listed for easy review, though the program always responds to these kinds of messages with a note that it wasn't understood and to call reception.

See [Troubleshooting](Troubleshooting.md) for additional information.

## Managing the room configurations

If the patient is physically attending the practice, they wait out in the car park until they arrive, and then they are summoned into the practice by a message that the doctor is ready to see them. The message might read something like this:

  Dr Adam Ant is ready to see you now. Please come to room 5
  
This saves or reduces the need for the patient interacting with (and maybe waiting at) reception to find out where they should go, if they don't already know.

However the PMS systems do not track which room the doctor is in. So this information must come from the application itself. The Room Mappings tab contains a list of doctors who have appointments for the day, and a text note that explains where to go for that doctor's office. If no note is configured for the doctor, the message will say:

  Dr Adam Ant is ready to see you now

## Performing Initial Configuration 

This screen handles configuration when the program is first set up, and shouldn't require any additional management after that.

See:

* [Setting up the SMS phone # using Twilio](Twilio.md)
* [Program Settings](Settings.md)

## Testing the Behaviour of the Application.

Because the application is glue between 3 different systems, it can be very difficult to understand how it works. To help explain how it works, and test it, the application can be run in simulator mode. 

See [Simulator Mode](Simulator.md) for further details.
