# Installing Clinic Arrivals

# How to install

* The application is distributed as a zipped binary through the GitHub release mechanism - see https://github.com/grahamegrieve/ClinicArrivals/releases
* Download the latest release
* the actual download is a zipped folder that is the executable for the program.
* copy the program into your preferred location (typically, c:\program files\ClnicalArrivals)
* create short cuts in the appropriate place(s) for ```ClinicArrivals.exe```, and the batch file ```Clinic Arrivals (Simulator).bat```
* It's often convenient to create a short cut to $appdata$\roaming\ClinicArrivals where the program keeps it's settings, records, and log files for easy access

Then [configure the program](Settings.md), and set it to autostart there after. Usually, windows should be configured to autostart the program.

With regard to where to install, consider the following requirements:
* Once running, it is important to keep it running always 
* the program can only run once for any given PMS - do not run more than one copy (and there's no way to prevent this from happening in the program)
* There is one part of the application that needs regular (daily?) maintenance, which is where the reception staff tailor the invitation message to come side (see [the workflow](Workflow.md)) for each individual doctor. For this reason, the program should run somewhere where the reception staff have access to it

# Moving ClinicArrivals

It's possible to move the ClinicArrivals from one PC to another. It's important, when doing that, 
to move the contents from $appdata$\roaming\ClinicArrivals to the new computer (if necessary, unless it roams) 
before running on the new computer - that folder stores the current status of messaging with all the patients,
and if the program runs against a new local store, all the conversations will go back to the start (confusing for
the patients)