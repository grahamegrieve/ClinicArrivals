using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClinicArrivals.Models
{
    public interface IFhirAppointmentReader
    {
        /// <summary>
        /// Retrieve the set of appointments that have mobile numbers with a status of booked, arrived, or fulfilled
        /// And attach the processing status data from local storage too
        /// </summary>
        /// <param name="model"></param>
        System.Threading.Tasks.Task<List<PmsAppointment>> SearchAppointments(DateTime date, IList<DoctorRoomLabelMapping> roomMappings, IArrivalsLocalStorage storage);
    }
}
