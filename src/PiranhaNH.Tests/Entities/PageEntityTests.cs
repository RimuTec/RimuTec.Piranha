using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHibernate;
using NUnit.Framework;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Services;
using RimuTec.PiranhaNH.DataAccess;
using RimuTec.PiranhaNH.Repositories;
using RimuTec.PiranhaNH.Services;

namespace RimuTec.PiranhaNH.Entities
{
   [TestFixture]
   public class PageEntityTests : FixtureBase
   {
      public PageEntityTests()
      {
         SessionFactory = Database.CreateSessionFactory();
         _contentFactory = new ContentFactory(_services);
         PageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory), Module.Mapper);
      }

      [OneTimeSetUp]
      public void FixtureSetUp()
      {
         Initialize();
      }

      [Test]
      public async Task SerializeDeserialize()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         using var api = CreateApi();
         MyPage page = await MyPage.CreateAsync(api).ConfigureAwait(false);
         page.SiteId = siteId;
         page.Title = "Startpage";
         page.Text = "Welcome";
         page.IsHidden = true;
         page.Published = DateTime.Now;
         await PageRepository.Save(page).ConfigureAwait(false);
         var pageId = page.Id;
         using var session = SessionFactory.OpenSession();
         using var transaction = session.BeginTransaction();
         var pageEntities = session.Query<PageEntity>().Where(p => p.Id == pageId);
         var mapped = Module.Mapper.ProjectTo<PageEntity>(pageEntities).First();
         var data = JsonConvert.SerializeObject(mapped);
         var pageType = mapped.PageType;
         var type = pageType.GetType().FullName;
         transaction.Commit();
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

      private IPageRepository PageRepository {get;}
      private ISessionFactory SessionFactory { get; }
   }
}
