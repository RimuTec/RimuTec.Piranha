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
      public async Task GetAll_WithOnePage()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         using var api = CreateApi();
         MyPage page = await MakePage(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         var pageIds = await pageRepository.GetAll(siteId).ConfigureAwait(false);
         Assert.AreEqual(1, pageIds.Count());
         Assert.AreEqual(1, pageIds.Count(id => id == pageId));
      }

      [Test]
      public async Task GetById()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         MyPage page = await MakePage(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         Assert.AreNotEqual(Guid.Empty, pageId);
         var retrieved = await pageRepository.GetById<MyPage>(pageId).ConfigureAwait(false);
         Assert.AreEqual(pageId, retrieved.Id);
         Assert.AreEqual(siteId, retrieved.SiteId);
         Assert.AreEqual("Startpage", retrieved.Title);
         Assert.AreEqual("Welcome", retrieved.Text.Value);
      }

      [Test]
      public async Task GetById_RandomIdReturnsNull()
      {
         var pageId = Guid.NewGuid();
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var retrieved = await pageRepository.GetById<MyPage>(pageId).ConfigureAwait(false);
         Assert.IsNull(retrieved);
      }

      [Test]
      public async Task GetById_PageWithBlocks()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         MyPage page = await MakePageWithBlocks(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         var retrieved = await pageRepository.GetById<MyPage>(pageId).ConfigureAwait(false);
         Assert.AreEqual(2, retrieved.Blocks.Count);
      }

      [Test]
      public async Task Save_AfterChange()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         MyPage page = await MakePage(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         string newText = $"Welcome at {DateTime.Now}";
         page.Text = newText;
         await pageRepository.Save(page).ConfigureAwait(false);
         var retrieved = await pageRepository.GetById<MyPage>(pageId).ConfigureAwait(false);
         Assert.AreEqual(newText, retrieved.Text.Value);
      }

      [Test]
      public async Task Delete()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         MyPage page = await MakePage(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         await pageRepository.Delete(pageId).ConfigureAwait(false);
         var pageIds = await pageRepository.GetAll(siteId).ConfigureAwait(false);
         Assert.AreEqual(0, pageIds.Count());
         // TODO: assert that no orphan comments, blocks, fields, etc. are left.
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

      private async Task<MyPage> MakePage(Guid siteId)
      {
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         MyPage page;
         using var api = CreateApi();
         page = await MyPage.CreateAsync(api).ConfigureAwait(false);
         page.SiteId = siteId;
         page.Title = "Startpage";
         page.Text = "Welcome";
         page.IsHidden = true;
         page.Published = DateTime.Now;
         await pageRepository.Save(page).ConfigureAwait(false);
         return page;
      }

      private async Task<MyPage> MakePageWithBlocks(Guid siteId)
      {
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var page = await MakePage(siteId).ConfigureAwait(false);
         page.Blocks.Add(new Piranha.Extend.Blocks.TextBlock
         {
            Body = "Sollicitudin Aenean"
         });
         page.Blocks.Add(new Piranha.Extend.Blocks.TextBlock
         {
            Body = "Ipsum Elit"
         });
         await pageRepository.Save(page).ConfigureAwait(false);
         return page;
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
