using ClinicArrivals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals
{
    public class NLogAdapter : ILoggingService
    {
        public NLogAdapter()
        {
            _logger = new NLog.LogFactory().GetLogger("ClinicArrivals");
        }

        private NLog.Logger _logger;

        public void Log(int level, string message)
        {
            if (level == 2)
                _logger.Error(message);
            else
                _logger.Info(message);
            // _logger.Debug(message);
            // _logger.Warn(message);
        }
    }

    public class LogMessage
    {
        string Level { get; set; }
        string Message { get; set; }
    }
}
