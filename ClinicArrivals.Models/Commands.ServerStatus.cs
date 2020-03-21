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
        string _canExecuteInStatus;
        Action _execute;
        bool processing;
        public ServerStatusCommand(ServerStatus model, string canExecuteInStatus, Action execute)
        {
            _model = model;
            _canExecuteInStatus = canExecuteInStatus;
            _execute = execute;
            (model as INotifyPropertyChanged).PropertyChanged += ServerStatusCommand_PropertyChanged;
        }

        private void ServerStatusCommand_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CanExecuteChanged?.Invoke(null, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (processing)
                return false;
            if (_model.CurrentStatus != _canExecuteInStatus)
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
                // _model.ErrorMessage = ex.Message;
            }
            finally
            {
                processing = false;
                CanExecuteChanged?.Invoke(parameter, new EventArgs());
            }
        }
    }
}
