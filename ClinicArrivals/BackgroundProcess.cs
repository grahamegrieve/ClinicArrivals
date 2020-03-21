using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ClinicArrivals.Models
{
    public class BackgroundProcess
    {
        public BackgroundProcess(Settings settings, ServerStatus status, Dispatcher dispatcher, Func<Task> execute)
        {
            _settings = settings;
            _status = status;
            _dispatcher = dispatcher;
            _execute = execute;
            _status.CurrentStatus = "stopped";

            _status.Start = new ServerStatusCommand(_status, "stopped", () =>
            { Start(); });

            _status.Stop = new ServerStatusCommand(_status, "running", () =>
            { Stop(); });
        }
        Settings _settings;
        ServerStatus _status;
        Dispatcher _dispatcher;
        Func<Task> _execute;

        private System.Threading.Timer _poll;

        public void Start()
        {
            _poll = new System.Threading.Timer((o) =>
            {
                _dispatcher.Invoke(async () =>
                {
                    try
                    {
                        _status.CurrentStatus = "processing";
                        await _execute.Invoke();
                        if (_poll == null)
                            _status.CurrentStatus = "stopped";
                        else
                            _status.CurrentStatus = "running";
                    }
                    catch (Exception ex)
                    {
                        _status.CurrentStatus = $"Error: {ex.Message}";
                    }
                });
            }, null, 0, _settings.PollIntervalSeconds * 1000);
        }

        public void Stop()
        {
            if (_poll != null)
            {
                _poll.Dispose();
                _poll = null;
            }

            _dispatcher.Invoke(() =>
            {
                if (_status.CurrentStatus == "processing")
                    _status.CurrentStatus = "stopping";
                else
                    _status.CurrentStatus = "stopped";
            });
        }
    }
}
