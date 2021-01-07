using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Services;
using RimuTec.PiranhaNH.DataAccess;
using RimuTec.PiranhaNH.Services;

namespace RimuTec.PiranhaNH.Repositories
{
   public class SiteRepositoryTests : FixtureBase
   {
      public SiteRepositoryTests()
      {
         SessionFactory = Database.CreateSessionFactory();
      }

      [OneTimeSetUp]
      public void FixtureSetUp()
      {
         _contentFactory = new ContentFactory(_services);
         Initialize();
      }

      [SetUp]
      public async Task SetUp()
      {
         await DeleteAllSites().ConfigureAwait(false);
      }

      [Test]
      public async Task GetAll_ForNonExistentSite()
      {
         var siteId = Guid.NewGuid();
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var sites = await repository.GetAll().ConfigureAwait(false);
         Assert.AreEqual(0, sites.Count(s => s.Id == siteId));
      }

      [Test]
      public async Task Save_NewSite()
      {
         var internalId = Guid.NewGuid().ToString("N");
         var site = new Site
         {
            Description = $"Site description {internalId}",
            InternalId = internalId,
            Title = $"Title {internalId}"
         };
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
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
         var site = new Site
         {
            Description = $"Site description {internalId}",
            InternalId = internalId,
            Title = $"Title {internalId}"
         };
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
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
         var site = new Site
         {
            Description = $"Site description {internalId}",
            InternalId = internalId,
            Title = $"Title {internalId}"
         };
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         await repository.Save(site).ConfigureAwait(false);
         await repository.Delete(site.Id).ConfigureAwait(false);
         var sites = await repository.GetAll().ConfigureAwait(false);
         Assert.AreEqual(0, sites.Count(s => s.Id == site.Id));
      }

      [Test]
      public async Task Delete_WithRandomId()
      {
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         await repository.Delete(Guid.NewGuid()).ConfigureAwait(false);
         // Nothing to assert. Should ignore the request without throwing exception.
      }

      [Test]
      public async Task GetById_ExistingSite()
      {
         var internalId = Guid.NewGuid().ToString("N");
         var site = new Site
         {
            Description = $"Site description {internalId}",
            InternalId = internalId,
            Title = $"Title {internalId}"
         };
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
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
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var retrieved = await repository.GetById(someId).ConfigureAwait(false);
         Assert.Null(retrieved);
      }

      [Test]
      public async Task GetDefault_NoDefault()
      {
         var internalId = Guid.NewGuid().ToString("N");
         var site = new Site
         {
            Description = $"Site description {internalId}",
            InternalId = internalId,
            Title = $"Title {internalId}",
            IsDefault = false
         };
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         await repository.Save(site).ConfigureAwait(false);
         var defaultSite = await repository.GetDefault().ConfigureAwait(false);
         Assert.IsNull(defaultSite);
      }

      [Test]
      public async Task GetDefault_WithExistingDefault()
      {
         var internalId = Guid.NewGuid().ToString("N");
         var site = new Site
         {
            Description = $"Site description {internalId}",
            InternalId = internalId,
            Title = $"Title {internalId}",
            IsDefault = true
         };
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         await repository.Save(site).ConfigureAwait(false);
         var siteId = site.Id;
         var defaultSite = await repository.GetDefault().ConfigureAwait(false);
         Assert.AreEqual(siteId, defaultSite.Id);
         Assert.IsTrue(defaultSite.IsDefault);
      }

      [Test]
      public async Task SaveGetContent()
      {
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var internalId = Guid.NewGuid().ToString("N");
         var site = new Site
         {
            Description = $"Site description {internalId}",
            InternalId = internalId,
            Title = $"Title {internalId}",
            IsDefault = true,
            SiteTypeId = "MySiteContent"
         };
         var content = new MySiteContent
         {
            Header = "<p>Lorem ipsum</p>",
            Footer = "<p>Tellus Ligula</p>"
         };
         await repository.Save(site).ConfigureAwait(false);
         var siteId = site.Id;
         await repository.SaveContent(siteId, content).ConfigureAwait(false);
         var contentId = content.Id;
         var retrieved = await repository.GetContentById<MySiteContent>(contentId).ConfigureAwait(false);
         Assert.AreNotEqual(Guid.Empty, retrieved.Id);
         Assert.AreEqual(content.Header, retrieved.Header);
         Assert.AreEqual(content.Footer, retrieved.Footer);
      }

      [Test]
      public async Task SaveContent_CanHandleContentNull()
      {
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var internalId = Guid.NewGuid().ToString("N");
         var site = new Site
         {
            Description = $"Site description {internalId}",
            InternalId = internalId,
            Title = $"Title {internalId}",
            IsDefault = true,
            SiteTypeId = "MySiteContent"
         };
         await repository.Save(site).ConfigureAwait(false);
         var siteId = site.Id;
         var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.SaveContent<MySiteContent>(siteId, null).ConfigureAwait(false));
         Assert.AreEqual("Parameter 'content' cannot be null. [Location 201226-2334] (Parameter 'content')", ex.Message);
      }

      // Need PageRepository for the following test to work
      // [Test]
      // public async Task GetSitemap()
      // {
      //    var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
      //    var internalId = Guid.NewGuid().ToString("N");
      //    var site = new Site
      //    {
      //       Description = $"Site description {internalId}",
      //       InternalId = internalId,
      //       Title = $"Title {internalId}",
      //       IsDefault = true,
      //       SiteTypeId = "MySiteContent"
      //    };
      //    await repository.Save(site).ConfigureAwait(false);
      //    var page = new Page {
      //    }
      // }

      [PageType(Title = "PageType")]
      public class MyPage : Page<MyPage>
      {
         [Region]
         public TextField Text { get; set; }
      }

      [SiteType(Title = "SiteType")]
      public class MySiteContent : SiteContent<MySiteContent>
      {
         [Region]
         public HtmlField Header { get; set; }

         [Region]
         public HtmlField Footer { get; set; }
      }

      private async Task DeleteAllSites()
      {
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var sites = await repository.GetAll().ConfigureAwait(false);
         var listOfTasks = new List<Task>();
         foreach (var site in sites)
         {
            listOfTasks.Add(repository.Delete(site.Id));
         }
         await Task.WhenAll(listOfTasks).ConfigureAwait(false);
      }

      private void Initialize()
      {
         using var api = CreateApi();
         Piranha.App.Init(api);

         var builder = new PageTypeBuilder(api)
                 .AddType(typeof(MyPage));
         builder.Build();

         var siteBuilder = new SiteTypeBuilder(api)
             .AddType(typeof(MySiteContent));
         siteBuilder.Build();
      }

      private ISessionFactory SessionFactory { get; }
   }
}
