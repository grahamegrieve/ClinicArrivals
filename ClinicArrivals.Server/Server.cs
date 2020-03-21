using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicArrivals.Server
{
    public class Server
    {
        private Oridashi.Fhir.Host.Configuration configuration;
        private Oridashi.Fhir.Host.FhirHost host;
        private bool running = false;

        public void Start(bool UseExamples)
        {
            configuration = new Oridashi.Fhir.Host.Configuration()
            {
                ProfileName = "clinicarrivals",
                LicenseKey = "3B36E5E1AD7645C6AD18F7A1",
                Mode = Oridashi.Fhir.Host.Configuration.SelectSystem.AUTO,
                IsLive = !UseExamples,
            };

            new System.Threading.Thread(() =>
            {
                host = new Oridashi.Fhir.Host.FhirHost();
                configuration = host.Start(configuration);
                running = true;
            }).Start();
        }

        public void Stop()
        {
            if (host != null)
                host.Stop();

            running = false;
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
