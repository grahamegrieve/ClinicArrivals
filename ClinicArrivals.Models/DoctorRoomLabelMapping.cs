using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Models
{
    [AddINotifyPropertyChangedInterface]
    public class DoctorRoomLabelMapping
    {
        public DoctorRoomLabelMapping(string id, string msg)
        {
            this.PractitionerFhirID = id;
            this.LocationName = msg;
        }

        public DoctorRoomLabelMapping()
        {
        }

        /// <summary>
        /// FHIR ID of the Practitioner Record
        /// </summary>
        public string PractitionerFhirID { get; set; }

        /// <summary>
        /// Name of the practitioner (cached from the FHIR resource)
        /// </summary>
        public string PractitionerName { get; set; }

        /// <summary>
        /// e.g. Allocated room number
        /// If we can do a mapping on this - probably going to be local file mapping? (prac to Room)
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// Any additional 
        /// </summary>
        public string LocationDescription { get; set; }
    }
}
