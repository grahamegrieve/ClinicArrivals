using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Server
{
    public class Server
    {
        public delegate void StartedServer();
        public delegate void StoppedServer();

        private Oridashi.Fhir.Host.Configuration configuration;
        private Oridashi.Fhir.Host.FhirHost host;
        private bool running = false;

        public event StartedServer OnStarted;
        public event StoppedServer OnStopped;

        public async Task Start(bool UseExamples, string ProfileNameSetting, string LicenseKeySetting)
        {
            configuration = new Oridashi.Fhir.Host.Configuration()
            {
                ProfileName = ProfileNameSetting,
                LicenseKey = LicenseKeySetting,
                Mode = Oridashi.Fhir.Host.Configuration.SelectSystem.AUTO,
                IsLive = !UseExamples,
            };

            host = new Oridashi.Fhir.Host.FhirHost();
            await Task<Oridashi.Fhir.Host.Configuration>.Run( new Action(() => {
                configuration = host.Start(configuration);
                running = true;
                OnStarted?.Invoke();
            }));          
        }

        public async Task Stop()
        {
            await Task<Oridashi.Fhir.Host.Configuration>.Run(new Action(() =>
            {
                if (host != null)
                    host.Stop();


                running = false;
                OnStopped?.Invoke();
            }));


           
        }

        public bool IsRunning
        {
            get
            {
                return running;
            }
        }

        public string Url
        {
            get
            {
                if (running)
                    return "https://localhost.oridashi.com.au:" + configuration.AutoPort.ToString();

                return null;
            }
        }


        public string Token
        {
            get
            {
                if (!running)
                    return null;

                string s = "";
                s += Oridashi.Fhir.Host.Dstu4.FhirServer.Configuration.ProfileName;
                s += ":";
                s += CalculateMD5Hash(Oridashi.Fhir.Host.Dstu4.FhirServer.Configuration.BasicAuth);
                return "Basic " + System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(s));
            }
        }

        protected static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hash = md5.ComputeHash(inputBytes);
                // step 2, convert byte array to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                    sb.Append(hash[i].ToString("X2"));
                return sb.ToString();
            }
        }
    }
}
