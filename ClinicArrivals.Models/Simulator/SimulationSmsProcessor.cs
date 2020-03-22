using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClinicArrivals.Models
{
    [AddINotifyPropertyChangedInterface]
    public class SimulationSmsProcessor : ISmsProcessor
    {
        public void Initialize(Settings settings)
        {
        }

        public string NewMessageFrom { get; set; }
        public string NewMessageDetails { get; set; }

        public ObservableCollection<SmsMessage> ReceivedMessages { get; private set; } = new ObservableCollection<SmsMessage>();
        public ObservableCollection<SmsMessage> SentMessages { get; private set; } = new ObservableCollection<SmsMessage>();

        public ICommand ClearMessages { get; set; }
        public ICommand QueueIncomingMessage { get; set; }

        #region << ISmsProcessor interface implementation >>
        public void SendMessage(SmsMessage sendMessage)
        {
            SentMessages.Add(sendMessage);
            System.Diagnostics.Trace.WriteLine($"SendMessage: {sendMessage.phone} - {sendMessage.message}");
        }

        /// <summary>
        /// Receive the current messages from the Twilio sms gateway
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<SmsMessage>> ReceiveMessages()
        {
            List<SmsMessage> results = new List<SmsMessage>();
            results.AddRange(ReceivedMessages);
            ReceivedMessages.Clear();
            return Task.FromResult(results as IEnumerable<SmsMessage>);
        }
        #endregion
    }
}
