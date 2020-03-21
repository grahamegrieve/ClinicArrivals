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
            model.Fulfilled.Clear();
            // model.Fulfilled.CollectionChanged += Fulfilled_CollectionChanged;
            Dispatcher.Invoke(async () =>
            {
                // read the settings from storage
                model.Settings.CopyFrom(await model.Storage.LoadSettings());

                // read the room mappings from storage
                model.RoomMappings.Clear();
                foreach (var map in await model.Storage.LoadRoomMappings())
                    model.RoomMappings.Add(map);

                // reload any unmatched messages
                var messages = await model.Storage.LoadUnprocessableMessages(model.DisplayingDate);

                // Start the FHIR server
                MessageProcessing.StartServer(model.Settings.ExamplesServer);

                // check for any appointments
                await MessageProcessing.CheckAppointments(model);
            });
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
