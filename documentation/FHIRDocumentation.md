# FHIR Interface

The Application uses a FHIR interface to communicate with the local 
Practice Management System (PMS). When run in a GP practice, the application
automatically locates the appropriate PMS (Medical Director or Best Practice)
and runs it's own internal FHIR Server. 

Alternatively, a different FHIR Server may be provided as specified in 
the configuration. Any FHIR Server must conform to these expectations
documented here.

## Conceptual Design

The application is appointment based. The application uses Appointment, Patient, and Practitioner 
Resources, though it only performs operations on the Appointment resource.

## Querying for appointments 

The fundamental query is to make a list of appointments for a day. 

    GET [base]/Appointment?date=YYYY-MM-DD&_include=patient&_include=practitioner
    
Note that the application will scan for the current day and the next few future days (less often). 

The returns a list of appointments. The bundle should not be paged. The Patient 
and Practitioner resources are also included in the response 

### Appointment

* id
* patient reference
* practitioner reference
* time of start & end
* appointment type - valued with "http://hl7.org/au/fhir/CodeSystem/AppointmentType" code = "teleconsultation"
* status code:  booked | arrived | fulfilled
* comments

### Patient

* id 
* name (family + given)
* identifier - MRN (with MRN type)

### Practitioner

* id 
* name (family + given)

## Updating an Appointment 

The interface accepts a PUT on an appointment 

Only 3 fields can be changed, and only one change at at time:
* status from booked to arrived (when the patient SMSs that they are in the carparK)
* appending "Appointment URL: {{url}}" to the comments
* updating the appoint type to "http://hl7.org/au/fhir/CodeSystem/AppointmentType" code = "teleconsultation" or removing it 


## Running the FHIR interface

Start the provided DLL. This connects to the relevant PMS autoamtically and starts a FHIR interface.
The interface start function returns both the port that the FHIR server is running on, and also a 
GUID that must be used for making phone calls

