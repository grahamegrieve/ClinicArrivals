using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Models
{
    public class MessageTemplate
    {
        public MessageTemplate(string type, string template = null)
        {
            MessageType = type;
            Template = template;
        }

        /// <summary>
        /// The Type of message template
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// The actual template of the message (Razor or simple string replace)
        /// </summary>
        public string Template { get; set; }
    }
}
