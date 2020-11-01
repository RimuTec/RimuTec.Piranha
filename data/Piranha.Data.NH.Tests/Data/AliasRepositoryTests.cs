using System;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task GetAll_EmptyDatabase()
        {
            var siteId = Guid.NewGuid();
            var repository = new AliasRepository(SessionFactory);
            var aliases = await repository.GetAll(siteId).ConfigureAwait(false);
            Assert.AreEqual(0, aliases.Count());
        }

        private ISessionFactory SessionFactory { get; }
    }
}
