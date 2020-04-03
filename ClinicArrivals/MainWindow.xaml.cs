﻿using ClinicArrivals.Models;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

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
                if (model.Settings.AutoStartServices)
                {
                    model.serverStatuses.Oridashi.Status = ServerStatusEnum.Running;
                    model.ScanAppointments.Start();
                    model.ReadSmsMessage.Start();
                    model.ProcessUpcomingAppointments.Start();
                }
            });
        }
    }

    /// <summary>
    /// Opens <see cref="Hyperlink.NavigateUri"/> in a default system browser
    /// credit: https://stackoverflow.com/a/27609749/72944
    /// </summary>
    public class ExternalBrowserHyperlink : Hyperlink
    {
        public ExternalBrowserHyperlink()
        {
            RequestNavigate += OnRequestNavigate;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
