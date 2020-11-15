using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using Piranha.Models;
using RimuTec.PiranhaNH.DataAccess;

namespace RimuTec.PiranhaNH.Repositories
{
    public class SiteRepositoryTests
    {
        public SiteRepositoryTests()
        {
            SessionFactory = Database.CreateSessionFactory();
        }

        [SetUp]
        public async Task SetUp()
        {
            await DeleteAllSites().ConfigureAwait(false);
        }

        [Test]
        public async Task GetAll_ForNonExistentSite() {
            var siteId = Guid.NewGuid();
            var repository = new SiteRepository(SessionFactory);
            var sites = await repository.GetAll().ConfigureAwait(false);
            Assert.AreEqual(0, sites.Count(s => s.Id == siteId));
        }

        [Test]
        public async Task Save_NewSite()
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
            Assert.AreNotEqual(Guid.Empty, site.Id);
            Assert.AreNotEqual(site.Id, sites.First(s => s.InternalId == internalId));
        }

        [Test]
        public async Task Save_ExistingSite()
        {
            var internalId = Guid.NewGuid().ToString("N");
            var site = new Site {
                Description = $"Site description {internalId}",
                InternalId = internalId,
                Title = $"Title {internalId}"
            };
            var repository = new SiteRepository(SessionFactory);
            await repository.Save(site).ConfigureAwait(false);
            var siteId = site.Id;
            var modifiedDescription = site.Description + " modified";
            site.Description = modifiedDescription;
            await repository.Save(site).ConfigureAwait(false);
            var retrieved = await repository.GetById(site.Id).ConfigureAwait(false);
            Assert.AreEqual(siteId, retrieved.Id);
            Assert.AreEqual(modifiedDescription, retrieved.Description);
        }

        [Test]
        public async Task Delete()
        {
            var internalId = Guid.NewGuid().ToString("N");
            var site = new Site {
                Description = $"Site description {internalId}",
                InternalId = internalId,
                Title = $"Title {internalId}"
            };
            var repository = new SiteRepository(SessionFactory);
            await repository.Save(site).ConfigureAwait(false);
            await repository.Delete(site.Id).ConfigureAwait(false);
            var sites = await repository.GetAll().ConfigureAwait(false);
            Assert.AreEqual(0, sites.Count(s => s.Id == site.Id));
        }

        [Test]
        public async Task Delete_WithRandomId()
        {
            var repository = new SiteRepository(SessionFactory);
            await repository.Delete(Guid.NewGuid()).ConfigureAwait(false);
            // Nothing to assert. Should ignore the request without throwing exception.
        }

        [Test]
        public async Task GetById_ExistingSite()
        {
            var internalId = Guid.NewGuid().ToString("N");
            var site = new Site {
                Description = $"Site description {internalId}",
                InternalId = internalId,
                Title = $"Title {internalId}"
            };
            var repository = new SiteRepository(SessionFactory);
            await repository.Save(site).ConfigureAwait(false);
            var siteId = site.Id;
            Assert.AreNotEqual(Guid.Empty, siteId);
            var retrieved = await repository.GetById(siteId).ConfigureAwait(false);
            Assert.AreEqual(internalId, retrieved.InternalId);
        }

        [Test]
        public async Task GetById_WithRandomId()
        {
            var someId = Guid.NewGuid();
            var repository = new SiteRepository(SessionFactory);
            var retrieved = await repository.GetById(someId).ConfigureAwait(false);
            Assert.Null(retrieved);
        }

        [Test]
        public async Task GetDefault_NoDefault()
        {
            var internalId = Guid.NewGuid().ToString("N");
            var site = new Site {
                Description = $"Site description {internalId}",
                InternalId = internalId,
                Title = $"Title {internalId}",
                IsDefault = false
            };
            var repository = new SiteRepository(SessionFactory);
            await repository.Save(site).ConfigureAwait(false);
            var defaultSite = await repository.GetDefault().ConfigureAwait(false);
            Assert.IsNull(defaultSite);
        }

        [Test]
        public async Task GetDefault_WithExistingDefault()
        {
            var internalId = Guid.NewGuid().ToString("N");
            var site = new Site {
                Description = $"Site description {internalId}",
                InternalId = internalId,
                Title = $"Title {internalId}",
                IsDefault = true
            };
            var repository = new SiteRepository(SessionFactory);
            await repository.Save(site).ConfigureAwait(false);
            var siteId = site.Id;
            var defaultSite = await repository.GetDefault().ConfigureAwait(false);
            Assert.AreEqual(siteId, defaultSite.Id);
            Assert.IsTrue(defaultSite.IsDefault);
        }

        private async Task DeleteAllSites()
        {
            var repository = new SiteRepository(SessionFactory);
            var sites = await repository.GetAll().ConfigureAwait(false);
            var listOfTasks = new List<Task>();
            foreach(var site in sites) {
                listOfTasks.Add(repository.Delete(site.Id));
            }
            await Task.WhenAll(listOfTasks).ConfigureAwait(false);
        }

        private ISessionFactory SessionFactory { get; }
    }
}
