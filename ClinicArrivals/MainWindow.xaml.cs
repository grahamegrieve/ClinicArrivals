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
        System.Threading.Timer poll;
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
            model.Fulfilled.CollectionChanged += Fulfilled_CollectionChanged;
            Dispatcher.Invoke(async ()=> { await MessageProcessing.CheckAppointments(model); });


            poll = new System.Threading.Timer((o) =>
            {
                Dispatcher.Invoke(async () =>
                {
                    var amodel = DataContext as Model;
                    await MessageProcessing.CheckAppointments(amodel);
                });
            }, null, 0, 10000);
        }

        private void Fulfilled_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(var item in e.NewItems)
            {
                new System.Threading.Thread(() =>
                 {
                     if((item as PmsAppointment).ReadyToBeNotifiedToComeInside)
                        System.Windows.MessageBox.Show("SMS " + (item as PmsAppointment).PatientName + " " + (item as PmsAppointment).PatientMobilePhone);
                 }).Start();
            }
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
