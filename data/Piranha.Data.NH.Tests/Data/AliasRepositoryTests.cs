using System;
using System.Linq;
using NHibernate;
using NUnit.Framework;
using RimuTec.Piranha.Data.DataAccess;

namespace RimuTec.Piranha.Repositories
{
    [TestFixture]
    public class AliasRepositoryTests
    {
        public AliasRepositoryTests()
        {
            SessionFactory = Database.CreateSessionFactory();
        }

        [Test]
        [Ignore("Maps for NHibernate are missing at the moment.")]
        public void GetAll()
        {
            var siteId = Guid.NewGuid();
            var repository = new AliasRepository(SessionFactory);
            var aliases = repository.GetAll(siteId).GetAwaiter().GetResult();
            Assert.AreEqual(0, aliases.Count());
        }

        private ISessionFactory SessionFactory { get; }
    }
}
