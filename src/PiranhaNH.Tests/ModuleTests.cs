using System;
using NUnit.Framework;

namespace RimuTec.Piranha.Data.NH.Tests
{
    [TestFixture]
    public class ModuleTests
    {
        [Test]
        public void Author()
        {
            var module = new Module();
            Assert.AreEqual("RimuTec Ltd", module.Author);
        }

        [Test]
        public void Name()
        {
            var module = new Module();
            Assert.AreEqual("RimuTec.PiranhaNH", module.Name);
        }

        [Test]
        public void Version()
        {
            var jan2020 = new DateTime(2020,01,01);
            var daysSinceThen = Math.Round(DateTime.Now.Subtract(jan2020).TotalDays);
            var module = new Module();
            Assert.AreEqual($"0.9.{daysSinceThen}", module.Version);
        }

        [Test]
        public void Description()
        {
            var module = new Module();
            Assert.AreEqual("Data implementation for NHibnernate.", module.Description);
        }

        [Test]
        public void PackageUrl()
        {
            var module = new Module();
            Assert.AreEqual("tbd", module.PackageUrl);
        }

        [Test]
        public void IconUrl()
        {
            var module = new Module();
            Assert.AreEqual("tbd", module.IconUrl);
        }

        [Test]
        public void Init()
        {
            var module = new Module();
            module.Init();
        }
    }
}
