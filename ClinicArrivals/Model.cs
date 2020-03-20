using ClinicArrivals.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals
{
    public class Model : ArrivalsModel
    {
        public Model()
        {
#if DEBUG
            // This is some test data to assist in the UI Designer
            Expecting.Add(new PmsAppointment() { ArrivalTime="10:35am", PatientName = "Postlethwaite, Brian", PatientMobilePhone = "0423857505", ArrivalStatus = "Pending", AppointmentStartTime = "9:45am", PractitionerName = "Dr Nathan Pinskier" });
            Expecting.Add(new PmsAppointment() { ArrivalTime = "10:35am", PatientName = "Esler, Brett", PatientMobilePhone = "0423083847", ArrivalStatus = "Pending", AppointmentStartTime = "10:00am", PractitionerName = "Dr Nathan Pinskier" });
            Waiting.Add(new PmsAppointment() { ArrivalTime = "10:35am", PatientName = "Grieve, Grahame", PatientMobilePhone = "0423083847", ArrivalStatus = "Arrived", AppointmentStartTime = "10:00am", PractitionerName = "Dr Nathan Pinskier" });
            RoomMappings.Add(new DoctorRoomLabelMappings() 
            {
                PractitionerFhirID = "1",
                PractitionerName = "Dr Nathan",
                LocationName = "Room 1",
                LocationDescription = "Proceed through the main lobby and its the 3rd door on the right"
            });
#endif
        }
    }
}
