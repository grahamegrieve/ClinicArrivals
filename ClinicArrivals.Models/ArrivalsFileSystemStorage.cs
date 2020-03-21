using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Models
{
    public class ArrivalsFileSystemStorage : IArrivalsLocalStorage
    {
        /// <summary>
        /// The optional path permits changing for testing purposes
        /// </summary>
        /// <param name="appDataFolder"></param>
        public ArrivalsFileSystemStorage(string appDataFolder = "ClinicArrivals")
        {
            _appDataFolder = appDataFolder;
        }
        string _appDataFolder;

        public string GetFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _appDataFolder);
        }

        private Task SaveFile<K>(string subFolder, string filename, K content)
        {
            string path = GetFolder();
            string filenameWithPath;
            if (!string.IsNullOrEmpty(subFolder))
                path = Path.Combine(GetFolder(), subFolder);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            filenameWithPath = Path.Combine(path, filename);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(content, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(filenameWithPath, json);
            return Task.CompletedTask;
        }

        private Task<K> LoadFile<K>(string subFolder, string filename, K defaultValue)
        {
            string pathname;
            if (!string.IsNullOrEmpty(subFolder))
                pathname = Path.Combine(GetFolder(), subFolder, filename);
            else
                pathname = Path.Combine(GetFolder(), filename);

            K result = defaultValue;
            if (File.Exists(pathname))
            {
                string json = File.ReadAllText(pathname);
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<K>(json);
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// Save the Room mappings to storage
        /// </summary>
        /// <param name="mappings"></param>
        /// <returns></returns>
        public Task SaveRoomMappings(IEnumerable<DoctorRoomLabelMappings> mappings)
        {
            return SaveFile(null, "room-mappings.json", mappings);
        }

        /// <summary>
        /// Load the room mappings from storage
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<DoctorRoomLabelMappings>> LoadRoomMappings()
        {
            return LoadFile<IEnumerable<DoctorRoomLabelMappings>>(null, "room-mappings.json", new List<DoctorRoomLabelMappings>());
        }

        /// <summary>
        /// Save the templates to storage
        /// </summary>
        /// <param name="templates"></param>
        /// <returns></returns>
        public Task SaveTemplates(IEnumerable<MessageTemplate> templates)
        {
            return SaveFile(null, "message-templates.json", templates);
        }

        /// <summary>
        /// Load the templates from storage
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<MessageTemplate>> LoadTemplates()
        {
            return LoadFile<IEnumerable<MessageTemplate>>(null, "message-templates.json", new List<MessageTemplate>());
        }


        /// <summary>
        /// Save that this received message was not able to be 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SaveUnprocessableMessage(string date, SmsMessage message)
        {
            return SaveFile(date, "unprocessable-messages.json", message);
        }


        /// <summary>
        /// In the event of a system shutdown, re-reading the unprocessed messages
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SmsMessage>> LoadUnprocessableMessages(string date)
        {
            return LoadFile<IEnumerable<SmsMessage>>(date, "unprocessable-messages.json", new List<SmsMessage>());
        }

        // TODO: Not sure if we actually need to store anything in here, maybe the actual message content that was received.
        // Could this go into a note in the Appointment itself as a write-back instead?
        public Task SaveAppointmentStatus(string date, PmsAppointment appt)
        {
            throw new NotImplementedException();
        }

        public Task LoadAppointmentStatus(string date, PmsAppointment appt)
        {
            throw new NotImplementedException();
        }

        #region << Settings >>
        public Task<Settings> LoadSettings()
        {
            return LoadFile(null, "appSettings.json", new Settings());
        }
        public Task SaveSettings(Settings settings)
        {
            return SaveFile(null, "appSettings.json", settings);
        }
        #endregion

        #region << Storage Maintenance Operations >>
        /// <summary>
        /// Cleanup any temporary or historic files
        /// </summary>
        /// <returns></returns>
        public Task CleanupHistoricCont()
        {
            foreach (var folder in Directory.EnumerateDirectories(GetFolder()))
            {
                Directory.Delete(folder);
            }
            return Task.CompletedTask;
        }
        #endregion
    }
}
