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
        public BackgroundProcess(Settings settings, ServerStatus status, Dispatcher dispatcher, Func<Task> execute, bool useRegisterPollInterval = false)
        {
            _settings = settings;
            _status = status;
            _dispatcher = dispatcher;
            _execute = execute;
            _status.Status = ServerStatusEnum.Stopped;
            _useRegisterPollInterval = useRegisterPollInterval;

            _status.Start = new ServerStatusCommand(_status, ServerStatusEnum.Stopped, () =>
            { Start(); });

            _status.Stop = new ServerStatusCommand(_status, ServerStatusEnum.Running, () =>
            { Stop(); });
        }
        Settings _settings;
        ServerStatus _status;
        Dispatcher _dispatcher;
        Func<Task> _execute;
        bool _useRegisterPollInterval;

        private System.Threading.Timer _poll;

        public void Start()
        {
            int intervalMS = _settings.PollIntervalSeconds * 1000;
            if (intervalMS == 0)
                intervalMS = 10 * 1000; // process every 10 seconds (if not specified)
            if (_useRegisterPollInterval)
            {
                intervalMS = _settings.RegistrationPollIntervalSeconds * 1000;
                if (intervalMS == 0)
                    intervalMS = 60 * 1000; // process every 1 minute (if not specified)
            }
            _poll = new System.Threading.Timer((o) =>
            {
                _dispatcher.Invoke(async () =>
                {
                    try
                    {
                        _status.Status = ServerStatusEnum.Processing;
                        await _execute.Invoke();
                        if (_poll == null)
                            _status.Status = ServerStatusEnum.Stopped;
                        else
                            _status.Status = ServerStatusEnum.Running;
                    }
                    catch (Exception ex)
                    {
                        _status.Status = ServerStatusEnum.Error;
                        _status.Error = ex.Message;
                    }
                });
            }, null, 0, intervalMS);
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
                if (_status.Status == ServerStatusEnum.Processing)
                    _status.Status = ServerStatusEnum.Stopping;
                else
                    _status.Status = ServerStatusEnum.Stopped;
            });
        }
    }
}
