using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using Piranha.Models;
using RimuTec.Piranha.Data.NH.DataAccess;
using RimuTec.Piranha.Data.NH.Repositories;

namespace RimuTec.Piranha.Data.NH.Repositories
{
    [TestFixture]
    public class AliasRepositoryTests
    {
        public AliasRepositoryTests()
        {
            SessionFactory = Database.CreateSessionFactory();
        }

        [Test]
        public async Task GetAll_ForSpecificNonExistingSite()
        {
            var siteId = Guid.NewGuid();
            var repository = new AliasRepository(SessionFactory);
            var aliases = await repository.GetAll(siteId).ConfigureAwait(false);
            Assert.AreEqual(0, aliases.Count());
        }

        [Test]
        public async Task SaveGetById_HappyPath()
        {
            Site siteModelRetrieved = await MakeSite().ConfigureAwait(false);

            var aliasModel = new Alias
            {
                SiteId = siteModelRetrieved.Id,
                AliasUrl = "blog.example.com",
                RedirectUrl = "www.example.com/blog",
                Type = RedirectType.Temporary
            };
            var aliasRepository = new AliasRepository(SessionFactory);
            await aliasRepository.Save(aliasModel).ConfigureAwait(false);
            var aliases = await aliasRepository.GetAll(siteModelRetrieved.Id).ConfigureAwait(false);
            Assert.AreEqual(1, aliases.Count());
        }

        private async Task<Site> MakeSite()
        {
            var siteInternalId = $"{Guid.NewGuid()}";
            var siteRepository = new SiteRepository(SessionFactory);
            var siteModelToCreate = new Site
            {
                InternalId = siteInternalId,
                Description = "Site 42",
                Title = "Title 42",
            };
            await siteRepository.Save(siteModelToCreate).ConfigureAwait(false);
            return await siteRepository.GetByInternalId(siteInternalId).ConfigureAwait(false);
        }

        private ISessionFactory SessionFactory { get; }
    }
}
