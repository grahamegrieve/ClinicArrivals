using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    [TestClass]
    public class TestFileSystemStorage
    {
        [TestMethod]
        public void SaveLoadSettings() 
        {
            // delete any pre-existing settings file
            var storage = new ClinicArrivals.Models.ArrivalsFileSystemStorage("ClinicArrivals.Test");
            string pathSettings = Path.Combine(storage.GetFolder(), "appSettings.json");
            if (File.Exists(pathSettings))
                File.Delete(pathSettings);

            // read a non existent settings file
            var settings = storage.LoadSettings().GetAwaiter().GetResult();
            Assert.IsNotNull(settings);

            // Put in some values
            settings.ACCOUNT_SID = "ACaab8ee23cacbf7f30da842053d91f3aa";
            settings.AUTH_TOKEN = "fcebb79799bc2d4a132802580763ae17";
            settings.FromTwilioMobileNumber = "+12678434041";
            settings.PollIntervalSeconds = 30;
            settings.IntroSmsMessage = "Velcome to the test of ClinicArrivals";
            storage.SaveSettings(settings);

            // Now re-read the settings 
            settings = storage.LoadSettings().GetAwaiter().GetResult();
            Assert.IsNotNull(settings);
            Assert.AreEqual(30, settings.PollIntervalSeconds);
            Assert.AreEqual("Velcome to the test of ClinicArrivals", settings.IntroSmsMessage);

            // Update some values and re-save
            settings.PollIntervalSeconds = 60;
            settings.IntroSmsMessage = "Welcome to the test of ClinicArrivals";
            settings.Save = new SaveSettingsCommand(storage);
            storage.SaveSettings(settings);

            // Now re-read the settings 
            settings = storage.LoadSettings().GetAwaiter().GetResult();
            Assert.IsNotNull(settings);
            Assert.AreEqual(60, settings.PollIntervalSeconds);
            Assert.AreEqual("Welcome to the test of ClinicArrivals", settings.IntroSmsMessage);
            Assert.IsNull(settings.Save);

            // Cleanup the test folder
            Directory.Delete(storage.GetFolder(), true);
        }

        [TestMethod]
        public async Task SaveLoadTemplates()
        {
            // delete any pre-existing settings file
            var storage = new ClinicArrivals.Models.ArrivalsFileSystemStorage("ClinicArrivals.Test");
            string pathSettings = Path.Combine(storage.GetFolder(), "message-templates.json");
            if (File.Exists(pathSettings))
                File.Delete(pathSettings);

            // read a non existent settings file
            var templates = (await storage.LoadTemplates())?.ToList();
            Assert.IsNotNull(templates);

            // Add in a template
            templates.Add(new MessageTemplate("Intro"));
            templates.Add(new MessageTemplate("PleaseWait"));
            templates.Add(new MessageTemplate("ComeInside"));
            await storage.SaveTemplates(templates);

            // Check that they are in there
            templates = (await storage.LoadTemplates())?.ToList();
            Assert.AreEqual(3, templates.Count, "Expected the same number of templates to come out");

            // append another
            templates.Add(new MessageTemplate("ComeInside"));
            await storage.SaveTemplates(templates);
            templates = (await storage.LoadTemplates())?.ToList();
            Assert.AreEqual(4, templates.Count, "Expected the same number of templates to come out");

            // Update the contents of a template
            string TemplateContent = "this is the template";
            templates[2].Template = TemplateContent;

            await storage.SaveTemplates(templates);
            templates = (await storage.LoadTemplates())?.ToList();
            Assert.AreEqual(4, templates.Count, "Expected the same number of templates to come out");
            Assert.AreEqual(TemplateContent, templates[2].Template, "The updated template content was not the same");

            // Cleanup the test folder
            Directory.Delete(storage.GetFolder(), true);
        }
    }
}
