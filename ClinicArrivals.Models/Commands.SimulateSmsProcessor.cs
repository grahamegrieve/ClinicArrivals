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
    public class SimulateProcessorCommand : ICommand
    {
        SimulationSmsProcessor _model;
        bool processing;
        public SimulateProcessorCommand(SimulationSmsProcessor model)
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
                if (str == "ClearMessages")
                {
                    if (!_model.ReceivedMessages.Any() && _model.SentMessages.Any())
                        return false;
                }
                if (str == "QueueIncomingMessage")
                {
                    if (string.IsNullOrEmpty(_model.NewMessageDetails))
                        return false;
                    if (string.IsNullOrEmpty(_model.NewMessageFrom))
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
                    if (str == "ClearMessages")
                    {
                        _model.ReceivedMessages.Clear();
                        _model.SentMessages.Clear();
                    }
                    if (str == "QueueIncomingMessage")
                    {
                        _model.ReceivedMessages.Add(new SmsMessage(_model.NewMessageFrom, _model.NewMessageDetails));
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
