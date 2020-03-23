# Using the Simulator

In order to help explain the program, and to allow testing, 
the program runs a simulator mode. 

# Simulator mode 

To use the simulator, run the application with the command line 
parameter -simulator 

In this mode, the application does not connect to the 
PMS or to Twilio to send real SMS messages. Instead, 
2 additional tabs display, for creating and managing 
appointments (PMS Simulator) and sending and receiving
SMS messages (Sms Simulator)

# Sms Simulator

This has a list of messages that the application has sent,
and an input box where you can send messages as if they were 
sent from any mobile number.

For mobile number, always use the full form e.g. 
```+61411555555``` with no spaces. 

# PMS Simulator

This allows you to create an edit appointments using a very simple interface 
as if they were being created by a Practice Management System. 

You can add appointments, select existing appointments, or delete them. 
The application will generate SMS messaes etc as if they are real 
appointments - which will appear in the SMS simulator, and responses 
entered as incoming messages will update the appointment status accordingly.

One common reason for selecting an appointment and updating is to move 
an appointment from ```arrived``` to ```fulfilled``` to trigger the 
message to please come in.

Field Documentation:
* **Patient Name**: 
* **Mobile Phone**: The mobile phone for the patient. No specific format requirement but use the formats as approved for data entry in the (imaginary) PMS - typically something like 0411 555 555
* **Practitioner Name**: The name of the practitioner. Note that the application keeps track of the names and assigns them identifiers in the background, which drives the Room Mapping Entries 
* **Appointment Date**: The date of the appointment in dd-mmm yyyy format. There's generally no reason to create appointments for testing for anything but today or tomorrow
* **Appointment Time**: The time of the appointment in hh:mm format 
* **Arrival Status**: One of ```Booked```, ```Arrived```, ```Fulfilled```. There's generally no point creating new appointments with a value of anything other than "Booked"

The check box is for whether the appointment has been labelled as a video appointment or not by the SMS workflow 








