# Settings

## SMS Message Settings

* **Twilio Account SID**: The Twilio Account id
* **Twilio Auth Token**: The Twilio API Token
* **From Phone Number**: The Twilio Phone number for the SMS services 

The first 3 settings come from your Twilio account - see [Twilio Setup](Twilio.md). The phone number is the number that represents 
the clinic. It must use the standard format (e.g. +6140155555)

* **Adminstrator SMS**: An SMS number that gets SMSed when ever the program starts or stops (but not when it crashes unexpectedly)
* **Mobile Phone White List**: A list of mobile phone numbers that the application will send messages to; other appointments are ignored 

The Mobile phone white list is provided so that clinics can set up the application against the live PMS and twilio and then test for a restricted set of numbers (sys admins etc)

## Video

* **Use Video Conferencing**: Whether to offer video consultations (if this is false, the application just manages patient arrivals)
* **Video Conferencing Manager**: The style of the video conferencing supported (at present only openVidu is supported, which is what is used if nothing is specified)
* **Video Service Password** The password for the control API of the video service for openVidu

You get the last value from the system administrator for https://video.healthintersections.com.au

## PMS 

* **Profile Name**: / **License Key**: Needed for Best Practice sites. 
* **Use Sample Server**: .... to do....

The values for  Profile Name: / License Key can be got from ... someone who knows (?not sure yet).

## Engine

A series of settings that control how the engine that drives the application works. 

* **Poll Interval**: How often to look for new appointments for today (default: 10secs)
* **Registration Poll Interval**: How often to look ifor appointments for the next 2 days (not such a hurry to send these out, so do this less often) (default 1 min)
* **Screening Message (min)**: How many minutes before the appointment to send the message that asks for teleconsultation vs physical visit (default 180min)
* **Video Invitation (min)**: How many minutes before the appointment to send the video invitation (default 10min)
* **Start Automatically**: Whether to start the services automatically when the program runs - set this to true once all the other settings are configured correctly

## Support

For Support...  see [Support](Support.md)

## Todo

These settings don't actually exist yet

FHIR Server:
Auth Token: 

If these are blank, the program will automatically connect to the local PMS 
(Medical Director or Best Practice) and use that. If these are populated,
the Server must conform to the [FHIR Documentation](FHIRDocumentation.md)
