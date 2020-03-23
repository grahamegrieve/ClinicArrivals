using ClinicArrivals.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Models
{
    [TestClass]
    public class MessageTemplateTester
    {
        [TestMethod]
        public void testValidation1()
        {
            string error;
            Assert.IsTrue(MessageTemplate.IsValid(MessageTemplate.MSG_APPT_READY, "This is the name: {{Patient.name}}", out error));
        }

        [TestMethod]
        public void testValidation2()
        {
            string error;
            Assert.IsTrue(MessageTemplate.IsValid(MessageTemplate.MSG_APPT_READY, "This is the name: {{Patient.name}} for {{Practitioner.name}}", out error));
        }

        [TestMethod]
        public void testValidation3()
        {
            string error;
            Assert.IsFalse(MessageTemplate.IsValid(MessageTemplate.MSG_APPT_READY, "This is the name: {{Patient.name1}}", out error));
        }
        [TestMethod]
        public void testValidation4()
        {
            string error;
            Assert.IsFalse(MessageTemplate.IsValid(MessageTemplate.MSG_APPT_READY, "This is the name: {{Patient.name}", out error));
        }
        [TestMethod]
        public void testValidation5()
        {
            string error;
            Assert.IsFalse(MessageTemplate.IsValid(MessageTemplate.MSG_APPT_READY, "This is the name{{: {{Patient.name}} }}", out error));
        }

    }
}
