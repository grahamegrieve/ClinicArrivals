# Twilio Setup

The application uses Twilio to send and receive SMS messages. You must set up a Twilio Account. 

## Twilio Account set up instructions

* Set up a Twilio Account https://www.twilio.com/sms/pricing/au
* Buy an Australian phone number. The only capability that matters is SMS
* Mark the number as a business number 
* Go through the tiresome number regulatory process and wait for approval
* Assign regulatory bundle and address and buy the SMS number 
* Give the number a friendly name (the name of your clinic)
* Configure the messaging:
  * A MESSAGE COMES in: WebHook -> https://clinics.healthintersections.com.au/twilio
  * PRIMARY HANDLER FAILS - leave blank
  * Save your changes 
  
For the app, you need 3 pieces of data from Twilio

* your mobile phone number, with no spaces: e.g. +67371421538 
* from the Settings/General page in Twilio:
  * your Account Sid: e.g. AC4201fe2cbf7f3b8e03aa9a8cad53d3aa
  * Your Auth Token (press the eye icon): e.g. 9f31b6f2a5d31397aa5d2626512691a7

Enter these 3 values in the App in the [settings](Settings.md)

Note: Those 3 example values above are not real values and won't actually work - you need your own values
