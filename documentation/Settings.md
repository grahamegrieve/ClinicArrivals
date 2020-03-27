# Settings

## SMS Message Settings

* **Twilio Account SID**: The Twilio Account ID
* **Twilio Auth Token**: The Twilio API Token
* **From Phone Number**: The Twilio Phone number for the SMS services 

The first 3 settings come from your Twilio account - see [Twilio Setup](Twilio.md). The phone number is the number that represents the clinic. It must use the full standard format (e.g. `+6140155555`).

* **Administrator SMS**: An SMS number that gets a message when ever the program starts or stops (but not when it crashes unexpectedly)
* **Mobile Phone White List**: A comma-separated list of mobile phone numbers that the application will send messages to; other appointments are ignored 

The mobile phone whitelist is provided so that clinics can set up the application against the live PMS and Twilio and then test for a restricted set of numbers (sys admins etc).

## Video

* **Use Video Conferencing**: Whether to offer video consultations. If unchecked, the application just manages patient arrivals
* **Video Conferencing Manager**: The style of the video conferencing supported (at present only OpenVidu is supported, which is what is used if nothing is specified)
* **Video Service Password** The password for the control API of the video service for OpenVidu

You get the last value from the system administrator for https://video.healthintersections.com.au

## PMS (Practice Management System) 

* **Profile Name**: / **License Key**: Needed for Best Practice sites. 
* **Use Sample Server**: .... to do....

The values for  Profile Name: / License Key can be obtained from ... someone who knows (?not sure yet).

## Engine

A series of settings that control how the engine that drives the application works. 

* **Poll Interval**: How often to look for new appointments for today (default if value is 0: 10secs)
* **Registration Poll Interval**: How often to look for appointments for the next 2 days (not such a hurry to send these out, so do this less often) (default if value is 0: 1 min)
* **Screening Message (min)**: How many minutes before the appointment to send the message that asks for teleconsultation vs physical visit (default if value is 0: 180min)
* **Video Invitation (min)**: How many minutes before the appointment to send the video invitation (default if value is 0: 10min)
* **Start Automatically**: Whether to start the services automatically when the program runs - enable this once all the other settings are configured correctly

## Support

For Support...  see [Support](Support.md)

## Todo

These settings don't actually exist yet

FHIR Server:
Auth Token: 

If these are blank, the program will automatically connect to the local PMS 
(Medical Director or Best Practice) and use that. If these are populated,
the Server must conform to the [FHIR Documentation](FHIRDocumentation.md)
