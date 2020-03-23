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
    public class SimulatePmsCommand : ICommand
    {
        SimulationPms _model;
        bool processing;
        public SimulatePmsCommand(SimulationPms model)
        {
            _model = model;
            (model as INotifyPropertyChanged).PropertyChanged += Model_PropertyChanged;
        }
        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // This is OK just to fire on any change, as only 1 property changes, and it is the status
            CanExecuteChanged?.Invoke(null, new EventArgs());
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (parameter is string str)
            {
                if (str == "CreateNewAppointment")
                {
                    // no special conditions here
                }
                if (str == "UpdateAppointment")
                {
                    if (_model.SelectedAppointment == null)
                        return false;
                }
                if (str == "DeleteAppointment")
                {
                    if (_model.SelectedAppointment == null)
                        return false;
                }
            }
            return true;
        }

        public void Execute(object parameter)
        {
            processing = true;
            try
            {
                if (parameter is string str)
                {
                    if (str == "CreateNewAppointment")
                    {
                        _model.ExecuteCreateNewAppointment();
                    }
                    if (str == "UpdateAppointment")
                    {
                        _model.ExecuteUpdateAppointment();
                    }
                    if (str == "DeleteAppointment")
                    {
                        _model.Appointments.Remove(_model.SelectedAppointment);
                        _model.Storage.SaveSimulationAppointments(_model.Appointments);
                        _model.SelectedAppointment = null;
                    }
                }
            }
            catch (Exception ex)
            {
                new NLog.LogFactory().GetLogger("ClinicArrivals").Error("Exception Simulating SMS Processor: " + ex.Message);
            }
            finally
            {
                processing = false;
                CanExecuteChanged?.Invoke(parameter, new EventArgs());
            }
        }
    }
}
