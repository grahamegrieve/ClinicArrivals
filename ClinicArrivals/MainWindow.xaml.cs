using ClinicArrivals.Models;
using System;
using System.Windows;

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
                DataContext = new ViewModel();
            var model = DataContext as ViewModel;
            model.Waiting.Clear();
            model.Expecting.Clear();
            Dispatcher.Invoke(async () =>
            {
                await model.Initialize(Dispatcher);

                // Start the FHIR server
                MessageProcessing.OnStarted += MessageProcessing_OnStarted;
                MessageProcessing.OnStopped += MessageProcessing_OnStopped;
                MessageProcessing.OnVisitStarted += MessageProcessing_OnVisitStarted;
                MessageProcessing.StartServer(model.Settings.ExamplesServer);

                model.serverStatuses.Oridashi.CurrentStatus = "starting...";
                model.serverStatuses.Oridashi.Start = new ServerStatusCommand(model.serverStatuses.Oridashi, "stopped", () =>
                {
                    model.serverStatuses.Oridashi.CurrentStatus = "starting...";
                    MessageProcessing.StartServer(model.Settings.ExamplesServer);
                });
                model.serverStatuses.Oridashi.Stop = new ServerStatusCommand(model.serverStatuses.Oridashi, "running", async () =>
                {
                    model.serverStatuses.Oridashi.CurrentStatus = "stopping...";
                    await MessageProcessing.StopServer();
                });
            });
        }

        private void MessageProcessing_OnVisitStarted(PmsAppointment appt)
        {
            System.Windows.MessageBox.Show("SMS TO " + appt.PatientName, "", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, MessageBoxOptions.ServiceNotification);
        }

        private void MessageProcessing_OnStopped()
        {
            Dispatcher.Invoke(() =>
            {
                var model = DataContext as ViewModel;
                model.serverStatuses.Oridashi.CurrentStatus = "stopped";
                model.ScanAppointments.Stop();
                model.ReadSmsMessage.Stop();
            });
        }

        private void MessageProcessing_OnStarted()
        {
            Dispatcher.Invoke(() =>
            {
                var model = DataContext as ViewModel;
                model.serverStatuses.Oridashi.CurrentStatus = "running";
                model.ScanAppointments.Start();
                // model.ReadSmsMessage.Start();
            });
        }

        // Manually perform a single Scan over the current appointments
        private async void buttonReadSms_Click(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ViewModel;
            var processor = new MessageProcessing();
            await processor.CheckForInboundSmsMessages(model);
        }

        private async void buttonScanAppointments_Click(object sender, RoutedEventArgs e)
        {
            var model = DataContext as ViewModel;
            await MessageProcessing.CheckAppointments(model);
        }
    }
}
