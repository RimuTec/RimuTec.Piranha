using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using Piranha.Models;
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

        [Test]
        public async Task Save()
        {
            var internalId = Guid.NewGuid().ToString("N");
            var site = new Site {
                Description = $"Site description {internalId}",
                InternalId = internalId,
                Title = $"Title {internalId}"
            };
            var repository = new SiteRepository(SessionFactory);
            await repository.Save(site).ConfigureAwait(false);
            var sites = await repository.GetAll().ConfigureAwait(false);
            Assert.AreEqual(1, sites.Count(s => s.InternalId == internalId));
        }

        private ISessionFactory SessionFactory { get; }
    }
}
