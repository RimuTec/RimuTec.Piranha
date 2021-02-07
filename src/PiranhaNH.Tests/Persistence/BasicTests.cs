using System;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using RimuTec.PiranhaNH.DataAccess;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.Persistence
{
   [TestFixture]
   public class BasicTests
   {
      [Test]
      public async Task Site()
      {
         using ISession session = _sessionFactory.OpenSession();
         using ITransaction txn = session.BeginTransaction();

         var siteTypeId = $"MySiteType-{Guid.NewGuid():N}";

         var siteType = new SiteTypeEntity()
         {
            Id = siteTypeId,
            Created = DateTime.Now,
            LastModified = DateTime.Now
         };
         var site = new SiteEntity
         {
            Culture = "en-NZ",
            Description = "My site",
            Hostnames = "example.org",
            InternalId = $"{Guid.NewGuid():N}",
            SiteTypeId = siteTypeId,
            Title = "Welcome to Example.org"
         };
         var siteId = await session.SaveAsync(site).ConfigureAwait(false);
         await txn.CommitAsync().ConfigureAwait(false);
      }

      [Test]
      public async Task SiteType()
      {
         var siteTypeId = $"MySiteType-{Guid.NewGuid():N}";
         using ISession session = _sessionFactory.OpenSession();
         using ITransaction txn = session.BeginTransaction();

         var siteType = new SiteTypeEntity()
         {
            Id = siteTypeId,
            Created = DateTime.Now,
            LastModified = DateTime.Now
         };
         var retrievedSiteTypeId = await session.SaveAsync(siteType).ConfigureAwait(false);
         Assert.AreEqual(siteTypeId, retrievedSiteTypeId);
         await txn.CommitAsync().ConfigureAwait(false);
      }

      [Test]
      public async Task PageType()
      {
         var pageTypeId = $"MyPageType-{Guid.NewGuid():N}";
         using ISession session = _sessionFactory.OpenSession();
         using ITransaction txn = session.BeginTransaction();

         var pageType = new PageTypeEntity
         {
            Id = pageTypeId,
            Created = DateTime.Now,
            LastModified = DateTime.Now
         };
         var retrievedPageTypeId = await session.SaveAsync(pageType).ConfigureAwait(false);
         await txn.CommitAsync().ConfigureAwait(false);
      }

      [Test]
      public async Task Page()
      {
         var siteTypeId = $"MySiteType-{Guid.NewGuid():N}";
         var pageTypeId = $"MyPageType-{Guid.NewGuid():N}";
         using ISession session = _sessionFactory.OpenSession();
         using ITransaction txn = session.BeginTransaction();

         var siteType = new SiteTypeEntity()
         {
            Id = siteTypeId,
            Created = DateTime.Now,
            LastModified = DateTime.Now
         };
         var site = new SiteEntity
         {
            Culture = "en-NZ",
            Description = "My site",
            Hostnames = "example.org",
            InternalId = $"{Guid.NewGuid():N}",
            SiteTypeId = siteTypeId,
            Title = "Welcome to Example.org"
         };
         var pageType = new PageTypeEntity
         {
            Id = pageTypeId,
            Created = DateTime.Now,
            LastModified = DateTime.Now
         };
         var page = new PageEntity{
            Created = DateTime.Now,
            LastModified = DateTime.Now,
            Title = "Home page",
            PageType = pageType,
            Site = site,
            ContentType = "Page"
         };
         var retrievedSiteTypeId = await session.SaveAsync(siteType).ConfigureAwait(false);
         var siteId = await session.SaveAsync(site).ConfigureAwait(false);
         var retrievedPageTypeId = await session.SaveAsync(pageType).ConfigureAwait(false);
         var pageId = await session.SaveAsync(page).ConfigureAwait(false);
         await txn.CommitAsync().ConfigureAwait(false);
      }

      private readonly ISessionFactory _sessionFactory = Database.CreateSessionFactory();
   }
}
