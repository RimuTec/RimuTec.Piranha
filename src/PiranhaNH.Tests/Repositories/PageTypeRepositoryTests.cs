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
   public class PageTypeRepositoryTests
   {
      public PageTypeRepositoryTests()
      {
         SessionFactory = Database.CreateSessionFactory();
      }

      [Test]
      public async Task SaveGetAll()
      {
         var repo = new PageTypeRepository(SessionFactory);
         var model = new PageType()
         {
            Id = $"PageType-{Guid.NewGuid():N}"
         };
         await repo.Save(model).ConfigureAwait(false);
         var models = await repo.GetAll().ConfigureAwait(false);
         Assert.AreEqual(1, models.Count(m => model.Id == m.Id));
      }

      [Test]
      public void SaveWithoutId()
      {
         var repo = new PageTypeRepository(SessionFactory);
         var model = new PageType();
         // Assert.ThrowsAsync<T>() example: https://stackoverflow.com/a/40030988/411428
         var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => repo.Save(model));
         Assert.AreEqual("Cannot be null or whitespace. [Code 210103-1645] (Parameter 'model.Id')", ex.Message);
      }

      [Test]
      public void SaveWithNull()
      {
         var repo = new PageTypeRepository(SessionFactory);
         var ex = Assert.ThrowsAsync<ArgumentNullException>(() => repo.Save(null));
         Assert.AreEqual("Cannot be null. [Code 210103-1659] (Parameter 'model')", ex.Message);
      }

      [Test]
      public async Task GetById()
      {
         var repo = new PageTypeRepository(SessionFactory);
         string pageTypeId = $"PageType-{Guid.NewGuid():N}";
         var model = new PageType()
         {
            Id = pageTypeId
         };
         await repo.Save(model).ConfigureAwait(false);
         var retrieved = await repo.GetById(pageTypeId).ConfigureAwait(false);
         Assert.AreEqual(pageTypeId, retrieved.Id);
      }

      [Test]
      public async Task GetById_RandomId()
      {
         var repo = new PageTypeRepository(SessionFactory);
         string pageTypeId = $"PageType-{Guid.NewGuid():N}";
         var retrieved = await repo.GetById(pageTypeId).ConfigureAwait(false);
         Assert.IsNull(retrieved);
      }

      private ISessionFactory SessionFactory { get; }
   }
}
