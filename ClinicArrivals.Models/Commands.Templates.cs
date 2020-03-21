using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicArrivals.Models
{
    public class SaveTemplatesCommand : ICommand
    {
        IArrivalsLocalStorage _storage;
        bool processing;
        public SaveTemplatesCommand(IArrivalsLocalStorage storage)
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
            if (parameter is IEnumerable<MessageTemplate> templates)
            {
                processing = true;
                try
                {
                    _storage.SaveTemplates(templates);
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

    public class ReloadTemplatesCommand : ICommand
    {
        IArrivalsLocalStorage _storage;
        bool processing;
        public ReloadTemplatesCommand(IArrivalsLocalStorage storage)
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
            if (parameter is ObservableCollection<MessageTemplate> templates)
            {
                processing = true;
                try
                {
                    var loadedTemplates = _storage.LoadTemplates().GetAwaiter().GetResult();
                    templates.Clear();
                    foreach (var template in loadedTemplates)
                        templates.Add(template);
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
}
