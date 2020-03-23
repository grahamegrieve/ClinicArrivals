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
        ArrivalsModel _model;
        bool processing;
        public SaveRoomMappingsCommand(ArrivalsModel model)
        {
            _model = model;
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
            if (parameter is IEnumerable<DoctorRoomLabelMapping> mappings)
            {
                processing = true;
                try
                {
                    _model.Storage.SaveRoomMappings(_model.EditRoomMappings);
                    _model.RoomMappings.Clear();
                    _model.RoomMappings.AddRange(_model.Storage.LoadRoomMappings().GetAwaiter().GetResult());
                }
                catch (Exception ex)
                {
                    new NLog.LogFactory().GetLogger("ClinicArrivals").Error("Exception Saving room mappings: " + ex.Message);
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
        ArrivalsModel _model;
        bool processing;
        public ReloadRoomMappingsCommand(ArrivalsModel model)
        {
            _model = model;
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
            processing = true;
            try
            {
                var loadedMappings = _model.Storage.LoadRoomMappings().GetAwaiter().GetResult();
                _model.RoomMappings.Clear();
                _model.RoomMappings.AddRange(loadedMappings);
                _model.EditRoomMappings.Clear();
                foreach (var room in loadedMappings)
                    _model.EditRoomMappings.Add(room);
            }
            catch (Exception ex)
            {
                new NLog.LogFactory().GetLogger("ClinicArrivals").Error("Exception Loading Room Mappings: " + ex.Message);
            }
            finally
            {
                processing = false;
                CanExecuteChanged?.Invoke(parameter, new EventArgs());
            }
        }
    }
}
