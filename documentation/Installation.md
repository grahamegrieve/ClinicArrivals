# Installing ClinicArrivals

# How to install

* The application is distributed as a zipped binary through the GitHub release mechanism - see https://github.com/grahamegrieve/ClinicArrivals/releases
* Download the latest release
* The actual download is a zipped folder that contains the executable for the program
* Extract the download into your preferred location (typically, C:\Program Files\ClinicArrivals)
* Create short cuts in the appropriate place(s) for ```ClinicArrivals.exe```, and the batch file ```Clinic Arrivals (Simulator).bat```
* It's often convenient to create a short cut to ```$appdata$\roaming\ClinicArrivals``` where the program keeps it's settings, records, and log files for easy access

Then [configure the program](Settings.md), and set it to autostart thereafter. Usually, Windows should be configured to autostart the program.

With regard to where to install, consider the following requirements:
* Once running, it is important to always keep it running
* There must only be one instance of the program for any given PMS (be careful to not run more than one copy as there's no way to prevent this from happening within the program)
* There is one part of the application that needs regular (daily?) maintenance, which is where the reception staff tailor the invitation message to come inside (see [the workflow](Workflow.md)) for each individual doctor. For this reason, the program should run somewhere the clinic reception staff have access to it.

# Moving ClinicArrivals

It's possible to move the ClinicArrivals from one PC to another. It's important, when doing that, to move the contents from ```$appdata$\roaming\ClinicArrivals``` to the new computer (manually, unless it roams with the user profile) before running on the new computer - that folder stores the current status of messaging with all the patients, and if the program runs against a new local store, all the conversations will go back to the start (which would be confusing for the patients).
