# FHIR Interface

The Application uses a FHIR R4 interface to communicate with the local Practice Management System (PMS).

In a GP practice setting with a compatible PMS (i.e. Medical Director or Best Practice), the application will stand up its own FHIR server in order to communicate with the PMS.

Alternatively, a different FHIR Server may be specified in the configuration. Any FHIR Server must conform to the expectations documented here.

## Conceptual Design

The application is appointment based. The application uses Appointment, Patient, and Practitioner 
Resources, though it only performs write operations on the Appointment resource.

## Querying for appointments 

The fundamental query is to make a list of appointments for a day:

    GET [base]/Appointment?date=YYYY-MM-DD&_include=patient&_include=practitioner
    
Note that the application will scan the current day and the next few future days (less often). 

The query shall return a list of appointments. The bundle should not be paged. The Patient and Practitioner resources are also included in the response.

The following resource properties are used by the application:

### Appointment

* id
* patient reference
* practitioner reference
* time of start & end
* appointment type - valued with "http://hl7.org.au/fhir/CodeSystem/AppointmentType" code = "teleconsultation"
* status code:  booked | arrived | fulfilled

In addition, the interface uses an extension: http://hl7.org.au/fhir/StructureDefinition/telehealth-videolink to represent the actual video link for the doctor to use to talk to the patient.

### Patient

* id 
* name (family + given)
* identifier - MRN (with MRN type)

### Practitioner

* id 
* name (family + given)

## Updating an Appointment 

The FHIR interface shall accept a PUT on an Appointment.

Only 3 fields can be changed, with only one change at a time:
* status from booked to arrived (when the patient SMSs that they are in the carpark)
* appending "Appointment URL: {{url}}" to the comments
* updating the appoint type to "http://hl7.org/au/fhir/CodeSystem/AppointmentType" code = "teleconsultation" or removing it 


## Running the FHIR interface

Start the provided DLL. This connects to the relevant PMS automatically and starts a FHIR interface.
The interface's start function returns both the port that the FHIR server is running on, as well as a GUID that must be used for making phone calls.
