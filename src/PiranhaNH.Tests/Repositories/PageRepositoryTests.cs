using System;
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
   [TestFixture]
   public class PageRepositoryTests : FixtureBase
   {
      public PageRepositoryTests()
      {
         SessionFactory = Database.CreateSessionFactory();
      }

      [OneTimeSetUp]
      public void FixtureSetUp()
      {
         _contentFactory = new ContentFactory(_services);
         Initialize();
      }

      [Test]
      public async Task GetAll_ExistingSite()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var pageIds = await pageRepository.GetAll(siteId).ConfigureAwait(false);
         Assert.AreEqual(0, pageIds.Count());
      }

      [Test]
      public async Task GetAll_NonExistingSite()
      {
         var someRandomId = Guid.NewGuid();
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var pageIds = await pageRepository.GetAll(someRandomId).ConfigureAwait(false);
         Assert.AreEqual(0, pageIds.Count());
      }

      [Test]
      public async Task GetAll_WithPage()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         using var api = CreateApi();
         var page = await MyPage.CreateAsync(api).ConfigureAwait(false);
         page.SiteId = siteId;
         page.Title = "Startpage";
         page.Text = "Welcome";
         page.IsHidden = true;
         page.Published = DateTime.Now;
         await pageRepository.Save(page).ConfigureAwait(false);
         var pageIds = await pageRepository.GetAll(siteId).ConfigureAwait(false);
         Assert.AreEqual(1, pageIds.Count());
      }

      private async Task<Guid> MakeSite()
      {
         var repository = new SiteRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var internalId = Guid.NewGuid().ToString("N");
         var site = new Site
         {
            Description = $"Site description {internalId}",
            InternalId = internalId,
            Title = $"Title {internalId}",
            IsDefault = true,
            SiteTypeId = "MySiteContent",
            Hostnames = "mysite.com"
         };
         await repository.Save(site).ConfigureAwait(false);
         return site.Id;
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

      private ISessionFactory SessionFactory { get; }
   }
}
