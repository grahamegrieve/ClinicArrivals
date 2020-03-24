# Settings

## SMS Message Settings

These settings come from your Twilio account - see [Twilio Setup](Twilio.md)

## PMS 

* Profile Name: / License Key: Needed for Best Practice sites. 
* Poll Interval: How often to look for new appointments for today 
* Registration Poll Interval: How often to look ifor appointments for the next 2 days (usually, not such a hurry to send these out)
* Use Sample Server: .... to do....

## Other 

* Mobile Phone White List: If this is empty, the program runs for all appointments. If one or a few mobile phone numbers are listed here, the application will ignore all appointments not for one of these phones. This is for testing purposes. The list is a comma seperated list of formal phone numbers (e.g. +61411555555)
* Video Conferencing Manager: Which video servive is in use. For now, only use OpenVidu.
* Video Service Password - the password for the program to use when interacting with the OpenVidu server

## Support

The values for Video Service Password, and BP Profile Name: / License Key cn be got from your installer.

## Todo

These settings don't actually exist yet

FHIR Server:
Auth Token: 

If these are blank, the program will automatically connect to the local PMS 
(Medical Director or Best Practice) and use that. If these are populated,
the Server must conform to the [FHIR Documentation](FHIRDocumentation.md)
