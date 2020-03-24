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
                FhirAppointmentReader.OnStarted += MessageProcessing_OnStarted;
                FhirAppointmentReader.OnStopped += MessageProcessing_OnStopped;
                FhirAppointmentReader.OnVisitStarted += MessageProcessing_OnVisitStarted;
                FhirAppointmentReader.StartServer(model.Settings.ExamplesServer, model.Settings.PMSProfileName, model.Settings.PMSLicenseKey);

                model.serverStatuses.Oridashi.Status = ServerStatusEnum.Starting;
                model.serverStatuses.Oridashi.Start = new ServerStatusCommand(model.serverStatuses.Oridashi, ServerStatusEnum.Stopped, () =>
                {
                    model.serverStatuses.Oridashi.Status = ServerStatusEnum.Starting;
                    FhirAppointmentReader.StartServer(model.Settings.ExamplesServer, model.Settings.PMSProfileName, model.Settings.PMSLicenseKey);
                });
                model.serverStatuses.Oridashi.Stop = new ServerStatusCommand(model.serverStatuses.Oridashi, ServerStatusEnum.Running, async () =>
                {
                    model.serverStatuses.Oridashi.Status = ServerStatusEnum.Stopping;
                    await FhirAppointmentReader.StopServer();
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
                model.serverStatuses.Oridashi.Status = ServerStatusEnum.Stopped;
                model.ScanAppointments.Stop();
                model.ReadSmsMessage.Stop();
            });
        }

        private void MessageProcessing_OnStarted()
        {
            Dispatcher.Invoke(() =>
            {
                var model = DataContext as ViewModel;
                model.serverStatuses.Oridashi.Status = ServerStatusEnum.Running;
                model.ScanAppointments.Start();
                // model.ReadSmsMessage.Start();
            });
        }
    }
}
