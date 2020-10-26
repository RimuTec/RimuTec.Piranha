using NUnit.Framework;
using RimuTec.Piranha.Data.NH;

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
    }
}
