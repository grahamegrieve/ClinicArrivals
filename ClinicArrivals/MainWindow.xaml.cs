using ClinicArrivals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClinicArrivals
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private System.Threading.Timer poll;

         public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (DataContext == null)
                DataContext = new Model();
            var model = DataContext as Model;
            model.Waiting.Clear();
            model.Expecting.Clear();
            Dispatcher.Invoke(async () =>
            {
                // read the settings from storage
                model.Settings.CopyFrom(await model.Storage.LoadSettings());
                if (model.Settings.SystemIdentifier == Guid.Empty)
                {
                    // this only occurs when the system hasn't had one allocated
                    // so we can create a new one, then save the settings.
                    // (this will force an empty setting file with the System Identifier if needed)
                    model.Settings.AllocateNewSystemIdentifier();
                    await model.Storage.SaveSettings(model.Settings);
                }

                // read the room mappings from storage
                model.RoomMappings.Clear();
                foreach (var map in await model.Storage.LoadRoomMappings())
                    model.RoomMappings.Add(map);

                // reload any unmatched messages
                var messages = await model.Storage.LoadUnprocessableMessages(model.DisplayingDate);

                // Start the FHIR server
                MessageProcessing.OnStarted += MessageProcessing_OnStarted;
                MessageProcessing.OnStopped += MessageProcessing_OnStopped;
                MessageProcessing.OnVisitStarted += MessageProcessing_OnVisitStarted;
                MessageProcessing.StartServer(model.Settings.ExamplesServer);


            });
        }

        private void MessageProcessing_OnVisitStarted(PmsAppointment appt)
        {
            System.Windows.MessageBox.Show("SMS TO " + appt.PatientName, "", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
        }

        private void MessageProcessing_OnStopped()
        {
            throw new NotImplementedException();
        }

        private void MessageProcessing_OnStarted()
        {
           poll = new System.Threading.Timer((o) =>
           {
               try
               {
                   Dispatcher.Invoke(async () =>
                   {
                       var model = DataContext as Model;
                       // check for any appointments
                       await MessageProcessing.CheckAppointments(model);
                   });
               }
               catch
               {

               }
           }, null, 0, 5000);
        }

        private async void buttonSmsOut_Click(object sender, RoutedEventArgs e)
        {
            var model = DataContext as Model;
            var processor = new MessageProcessing();
            await processor.CheckForMessages(model);
        }

        private async void buttonRefresh_Click(object sender, RoutedEventArgs e)
        {
            var model = DataContext as Model;
            await MessageProcessing.CheckAppointments(model);
        }
    }
}
