using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicArrivals.Models
{
    public class SaveSettingsCommand : ICommand
    {
        IArrivalsLocalStorage _storage;
        bool processing;
        public SaveSettingsCommand(IArrivalsLocalStorage storage)
        {
            _storage = storage;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (processing)
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is Settings settings)
            {
                processing = true;
                try
                {
                    _storage.SaveSettings(settings);
                }
                catch (Exception ex)
                {
                    new NLog.LogFactory().GetLogger("ClinicArrivals").Error("Exception Saving Settings: " + ex.Message);
                }
                finally
                {
                    processing = false;
                    CanExecuteChanged?.Invoke(parameter, new EventArgs());
                }
            }
        }
    }

    public class ReloadSettingsCommand : ICommand
    {
        IArrivalsLocalStorage _storage;
        bool processing;
        public ReloadSettingsCommand(IArrivalsLocalStorage storage)
        {
            _storage = storage;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (processing)
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is Settings settings)
            {
                processing = true;
                try
                {
                    var loadedSettings = _storage.LoadSettings().GetAwaiter().GetResult();
                    settings.CopyFrom(loadedSettings);
                }
                catch (Exception ex)
                {
                    new NLog.LogFactory().GetLogger("ClinicArrivals").Error("Exception Loading Settings: " + ex.Message);
                }
                finally
                {
                    processing = false;
                    CanExecuteChanged?.Invoke(parameter, new EventArgs());
                }
            }
        }
    }
}
