# ClinicArrivals 
 
This program should help a General Practitioner (GP) manage increased workload during the [COVID-19](https://en.wikipedia.org/wiki/Coronavirus_disease_2019) crisis in Australia. It is designed to fit into an existing clinic workflow using common Practice Management Systems (PMS).

* [Product documentation](documentation/Documentation.md)

## Contributions

Contributions are welcome, either as Pull Requests or bug reports. You can joint the chat at: 
https://chat.fhir.org/#narrow/stream/227888-clinic-arrivals

If you are a user, contact RACGP IT forum for advice/support.

## Developer Documentation

### Building

Requirements:
Visual Studio 2019 on Windows.

.NET Framework target: 4.6.2.

### Local Storage 

The application stores information locally in the $AppData$\Roaming\ClinicArrivals.

The current [settings](documentation/Settings.md) are stored there. 

In addition, the communication record of past appointments is also stored there. Deleting this information or moving the application to a different PC without moving this data will reset any ongoing messaging flows with the patients.

### Kernel

The core of the program is in `MessageEngine.cs`. This is where the application queries for the current appointment list from the PMS, and also scans for incoming SMS messages to process.

Periodically (as specified in the settings), the application searches for all appointments on the current day. Then it works through the appointments.


### Testing
