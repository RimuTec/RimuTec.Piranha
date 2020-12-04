using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using Piranha.Models;
using RimuTec.PiranhaNH.DataAccess;

namespace RimuTec.PiranhaNH.Repositories
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
        public async Task SaveGetAll_HappyPath()
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

        [Test]
        public async Task Save_SetsId()
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
            Assert.False(aliasModel.Id == Guid.Empty);
        }

        [Test]
        public async Task Delete()
        {
            var aliasId = Guid.NewGuid().ToString("N");
            Site siteModelRetrieved = await MakeSite().ConfigureAwait(false);
            var aliasModel = new Alias
            {
                SiteId = siteModelRetrieved.Id,
                AliasUrl = "blog.example.com",
                RedirectUrl = $"www.example.com/blog?id={aliasId}",
                Type = RedirectType.Temporary
            };
            var aliasRepository = new AliasRepository(SessionFactory);
            await aliasRepository.Save(aliasModel).ConfigureAwait(false);
            var aliases = await aliasRepository.GetAll(siteModelRetrieved.Id).ConfigureAwait(false);
            var toDelete = aliases.First(x => x.RedirectUrl.Contains(aliasId));
            await aliasRepository.Delete(toDelete.Id).ConfigureAwait(false);
            aliases = await aliasRepository.GetAll(siteModelRetrieved.Id).ConfigureAwait(false);
            Assert.False(aliases.Any(x => x.RedirectUrl.Contains(aliasId)));
        }

        [Test]
        public async Task GetById()
        {
            var aliasId = Guid.NewGuid().ToString("N");
            Site siteModelRetrieved = await MakeSite().ConfigureAwait(false);
            string redirectUrl = $"www.example.com/blog?id={aliasId}";
            var aliasModel = new Alias
            {
                SiteId = siteModelRetrieved.Id,
                AliasUrl = "blog.example.com",
                RedirectUrl = redirectUrl,
                Type = RedirectType.Temporary
            };
            var aliasRepository = new AliasRepository(SessionFactory);
            await aliasRepository.Save(aliasModel).ConfigureAwait(false);
            var retrievedAlias = await aliasRepository.GetById(aliasModel.Id).ConfigureAwait(false);
            Assert.AreEqual(aliasModel.Id, retrievedAlias.Id);
            Assert.AreEqual(redirectUrl, retrievedAlias.RedirectUrl);
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
