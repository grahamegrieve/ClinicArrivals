using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicArrivals.Models
{
    public class ServerStatusCommand : ICommand
    {
        ServerStatus _model;
        ServerStatusEnum _canExecuteInStatus;
        Action _execute;
        bool processing;
        public ServerStatusCommand(ServerStatus model, ServerStatusEnum canExecuteInStatus, Action execute)
        {
            _model = model;
            _canExecuteInStatus = canExecuteInStatus;
            _execute = execute;
            (model as INotifyPropertyChanged).PropertyChanged += ServerStatusCommand_PropertyChanged;
        }

        private void ServerStatusCommand_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // This is ok just to fire on any change, as only 1 property changes, and it is the status
            CanExecuteChanged?.Invoke(null, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (processing)
                return false;
            if (_model.Status != _canExecuteInStatus)
                return false;
            return true;
        }

        public void Execute(object parameter)
        {
            processing = true;
            try
            {
                _execute?.Invoke();
            }
            catch (Exception ex)
            {
                new NLog.LogFactory().GetLogger("ClinicArrivals").Error("Exception executing server command: " + ex.Message);
            }
            finally
            {
                processing = false;
                CanExecuteChanged?.Invoke(parameter, new EventArgs());
            }
        }
    }
}
