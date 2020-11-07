using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using RimuTec.Piranha.Data.NH.DataAccess;

namespace RimuTec.Piranha.Data.NH.Repositories
{
    public class SiteRepositoryTests
    {
        public SiteRepositoryTests()
        {
            SessionFactory = Database.CreateSessionFactory();
        }

        [Test]
        public async Task GetAll_ForNonExistentSite() {
            var siteId = Guid.NewGuid();
            var repository = new SiteRepository(SessionFactory);
            var sites = await repository.GetAll().ConfigureAwait(false);
            Assert.AreEqual(0, sites.Count(s => s.Id == siteId));
        }

        private ISessionFactory SessionFactory { get; }
    }
}
