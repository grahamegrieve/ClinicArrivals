# Security Considerations

## Desktop Application

* The desktop application needs direct access to the PMS application 
* it also needs web access to the following URLS: 
  * https://api.twilio.com
  * https://clinics.healthintersections.com.au
  * https://video.healthintersections.com.au (port 80 and port 4443)
  
* it does not perform the role of a server - there are no inbound message connections  
* the application only accesses clinic appointments - there's no access to any other part of the patient record

## SMS proxy server

* The Twilio SMS gateway uses a webhook to handle incoming SMS messages
* these are configured to be sent to https://clinics.healthintersections.com.au/twilio. The webhook includes the `AccountSid` - messages are filed under the account
* clients also query https://clinics.healthintersections.com.au/twilio and pass their account id as a parameter. The server returns all messages with a matching account id 
* most messages passing through the sms proxy will consist of one word answers: `yes | no | joined | arrived` (or variants) 
* the server itself is open source (see https://github.com/GrahameGrieve/FhirServer). It doesn't log messages or retain them more than a day after they have been processed.
* it's possible to run the server elsewhere, or replace it with another server that has the same interface 

## Video Calls

* Jitsi/OpenVidu are supported for video calls

Jitsi:
* users do not need an account or to sign up in any way
* the server https://meet.jit.si is used for video call coordination
* the actual video traffic flows peer to peer, not through a central server

OpenVidu:
[document me]

## SMS workflow

* messages are sent to the nominated mobile number for the patient using the current registered phone number 
* incoming messages from mobile phone numbers other than the registered phone, or even from a registered phone not associated with an appointment on the same day are considered as errors and ignored 
* if the messages can't be sent or aren't read, then the existing manual workflow will still apply


