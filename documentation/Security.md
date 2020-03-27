# Security Considerations

## Desktop Application

The ClinicArrivals application components run in the context of a normal user.

Communications requirements:
* outgoing access on local network to the PMS application using FHIR R4
* outgoing Internet access to the following URLS:
  * https://api.twilio.com
  * https://clinics.healthintersections.com.au
  * https://video.healthintersections.com.au (port 443 and 4443)
  
Risk mitigation:
* It does not perform the role of a server - there are no inbound message connections  
* The application only accesses clinic appointments - there's no access to any other part of the patient record

## SMS proxy server

* The Twilio SMS gateway uses a webhook to handle incoming SMS messages
* Webhooks are configured to be sent to https://clinics.healthintersections.com.au/twilio and include the `AccountSid` - messages are filed under the account ID
* Clients also query https://clinics.healthintersections.com.au/twilio and pass their account ID as a parameter - the server returns all messages with a matching account ID
* Most messages passing through the SMS proxy will consist of one word answers: `yes | no | joined | arrived` (or variants)
* The FHIR server itself is open source (see https://github.com/GrahameGrieve/FhirServer). It doesn't log messages or retain them for more than a day after they have been processed
* The FHIR server is hosted in AWS Asia Pacific (Sydney) - it's possible to run the server elsewhere, or replace it with another server that has the same interface

## Video Calls

* Both Jitsi and OpenVidu are implemented for video calls, but only OpenVidu is supported 

### OpenVidu

* An Ubuntu 16.04 server is hosted in AWS Asia Pacific (Sydney), deployed using open-source OpenVidu CloudFormation scripting which includes automatic security updates
* OpenVidu Server is available at https://video.healthintersections.com.au:4443
* Server-side components are available at https://video.healthintersections.com.au
* The session id is created on the fly by the ClinicArrivals program 
* Anyone who has the session id can join the call (this is a feature - it's easy to bring family members or translators into the call)
* Anyone who joins the call is visible to other parties in the call 
* The public API cannot create or iterate the sessions (actually, that's a todo)

### Jitsi

Note: Jitsi not currently supported.

* users do not need an account or to sign up in any way
* the server https://meet.jit.si is used for video call coordination
* the actual video traffic flows peer to peer, not through a central server


## SMS workflow

* messages are sent to the nominated mobile number for the patient using the current registered phone number 
* incoming messages from mobile phone numbers other than the registered phone, or even from a registered phone not associated with an appointment on the same day are considered as errors and ignored 
* if the messages can't be sent or aren't read, then the existing manual workflow will still apply


