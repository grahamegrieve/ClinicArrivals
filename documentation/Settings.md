# Settings

## SMS Message Settings

* **Twilio Account SID**: The Twilio Account ID
* **Twilio Auth Token**: The Twilio API Token
* **From Phone Number**: The Twilio Phone number for the SMS services 

The first 3 settings come from your Twilio account - see [Twilio Setup](Twilio.md). The phone number is the number that represents the clinic. It must use the full standard format (e.g. `+6140155555`).

* **Administrator SMS**: An SMS number that gets a message when ever the program starts or stops (but not when it crashes unexpectedly)
* **Mobile Phone White List**: A comma-separated list of mobile phone numbers that the application will send messages to; other appointments are ignored 
* **Message Limit per Number**: The upper limit to the number of messages that will be sent to a single phone number on a single day. This is intended to prevent sms storms where the recipient is also a program and they get stuck messaging each other with error messages (default if value is 0: 1 min)

The mobile phone whitelist is provided so that clinics can set up the application against the live PMS and Twilio and then test for a restricted set of numbers (sys admins etc).

## Video

* **Use Video Conferencing**: Whether to offer video consultations. If unchecked, the application just manages patient arrivals
* **Video Conferencing Manager**: The style of the video conferencing supported (at present only OpenVidu is supported, which is what is used if nothing is specified)
* **Video Service Password** The password for the control API of the video service for OpenVidu

You get the video service password from the system administrator for video.healthintersections.com.au - email admin@healthintersections.com.au (see below)

## PMS (Practice Management System) 

* **Profile Name**: / **License Key**: Needed for all installations to enable integration.
* **Use Sample Server**: .... to do....

The values for  Profile Name: / License Key can be obtained from the system administrator for video.healthintersections.com.au - email admin@healthintersections.com.au (see below)

For Zedmed v31 you will be prompted for the INTEGRATOR database password on starting up. This password is normally set on installation of Zedmed and may be changed in Zedmed security settings. Be aware changing this datbase password may affect other integrating software so care should be taken.

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

## Healthintersections.com.au - Services

The application depends on services provided by healthintersections.com.au. (video conferencing and SMS relaying). These
services are presently provided free of charge while the cost of providing the services is determined, and we figure out 
whether appropriate support is available. It's possible that it may be necessary to introduce a GoFundMe or require
clinics to run their own copy of these servers (they run open source services, but are not - at this time - straight 
forward to set up).

In order to access these services, you must email your agreement these terms and conditions to admin@healthintersections.com.au:


> I understand the services provided by healthintersections.com.au are provided on an as is basis, 
> in order to allow Australian GPs to respond urgently to the challenges provided by the COVID-19
> epidemic. I understand that the commercial arrangements that support the free provision of these 
> services is not yet fully worked out, and that Health Intersections Pty Ltd will continue to 
> work with Australian GPs and the RACGP to find sustainable ongoing arrangements. I understand
> that the services may cease to be provided and/or free at such a time as better options become 
> available. 
> 
> I agree that this service agreement is a transient one to make the services available, and 
> will be revised in the future based on legal advice. 
 
