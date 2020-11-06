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
        public async Task GetAll_EmptyDatabase()
        {
            var siteId = Guid.NewGuid();
            var repository = new AliasRepository(SessionFactory);
            var aliases = await repository.GetAll(siteId).ConfigureAwait(false);
            Assert.AreEqual(0, aliases.Count());
        }

        [Test]
        [Ignore("Not fully implemented yet.")]
        public async Task SaveGetById_NewAlias_HappyPath()
        {
            var siteId = Guid.NewGuid();
            var aliasModel = new Alias {
                SiteId = siteId,
                AliasUrl = "blog.example.com",
                RedirectUrl = "www.example.com/blog",
                Type = RedirectType.Temporary
            };
            var repository = new AliasRepository(SessionFactory);
            await repository.Save(aliasModel).ConfigureAwait(false);
            var aliases = await repository.GetAll(siteId).ConfigureAwait(false);
            Assert.AreEqual(1, aliases.Count());
        }

        private ISessionFactory SessionFactory { get; }
    }
}
