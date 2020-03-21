using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicArrivals.Models
{
    public class SaveRoomMappingsCommand : ICommand
    {
        IArrivalsLocalStorage _storage;
        bool processing;
        public SaveRoomMappingsCommand(IArrivalsLocalStorage storage)
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
            if (parameter is IEnumerable<DoctorRoomLabelMappings> mappings)
            {
                processing = true;
                try
                {
                    _storage.SaveRoomMappings(mappings);
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

    public class ReloadRoomMappingsCommand : ICommand
    {
        IArrivalsLocalStorage _storage;
        bool processing;
        public ReloadRoomMappingsCommand(IArrivalsLocalStorage storage)
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
            if (parameter is ObservableCollection<DoctorRoomLabelMappings> mappings)
            {
                processing = true;
                try
                {
                    var loadedMappings = _storage.LoadRoomMappings().GetAwaiter().GetResult();
                    mappings.Clear();
                    foreach (var room in loadedMappings)
                        mappings.Add(room);
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
