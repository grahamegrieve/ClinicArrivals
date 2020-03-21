using System.Collections.Generic;

namespace ClinicArrivals.Models
{
    public interface ITemplateProcessor
    {
        public string processTemplate(string templateId, Dictionary<string, object> variables);
    }
}