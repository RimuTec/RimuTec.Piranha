using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

      [Test]
      public async Task GetStartPage_EmptySite()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var startPage = await pageRepository.GetStartpage<DynamicPage>(siteId).ConfigureAwait(false);
         Assert.IsNull(startPage);
      }

      [Test]
      public async Task GetStartPage_SinglePage()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var aPage = await MakePage(siteId).ConfigureAwait(false);
         aPage.ParentId = null;
         var pageId = aPage.Id;
         await pageRepository.Save(aPage).ConfigureAwait(false);
         var startPage = await pageRepository.GetStartpage<DynamicPage>(siteId).ConfigureAwait(false);
         Assert.AreEqual(pageId, startPage.Id);
      }

      [Test]
      public async Task GetStartPage_BasedOnParent()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var secondPage = await MakePage(siteId).ConfigureAwait(false);
         secondPage.ParentId = null;
         var secondPageId = secondPage.Id;
         firstPage.ParentId = secondPageId;
         await pageRepository.Save(secondPage).ConfigureAwait(false);
         await pageRepository.Save(firstPage).ConfigureAwait(false);
         var retrieved = await pageRepository.GetStartpage<DynamicPage>(siteId).ConfigureAwait(false);
         Assert.AreEqual(secondPageId, retrieved.Id);
      }

      [Test]
      public async Task GetStartPage_BasedOnSortOrder()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var secondPage = await MakePage(siteId).ConfigureAwait(false);
         var secondPageId = secondPage.Id;
         firstPage.SortOrder = 1;
         secondPage.SortOrder = 0;
         await pageRepository.Save(secondPage).ConfigureAwait(false);
         await pageRepository.Save(firstPage).ConfigureAwait(false);
         var retrieved = await pageRepository.GetStartpage<DynamicPage>(siteId).ConfigureAwait(false);
         Assert.AreEqual(secondPageId, retrieved.Id);
      }

      [Test]
      public async Task GetAllComments()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var comments = await pageRepository.GetAllComments(firstPage.Id, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(0, comments.Count());
      }

      [Test]
      public async Task GetAllComments_RandomPageId()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var randomPageId = Guid.NewGuid();
         var comments = await pageRepository.GetAllComments(randomPageId, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(0, comments.Count());
      }

      [Test]
      public async Task GetAllComments_PageWithOneComment()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var comment = MakeComment();
         await pageRepository.SaveComment(firstPage.Id, comment).ConfigureAwait(false);
         var comments = await pageRepository.GetAllComments(firstPage.Id, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(1, comments.Count());
      }

      [Test]
      public async Task GetAllComments_2PagesWithComment()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));

         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         await pageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);

         var secondPage = await MakePage(siteId).ConfigureAwait(false);
         var secondComment = MakeComment();
         await pageRepository.SaveComment(secondPage.Id, secondComment).ConfigureAwait(false);

         var comments = await pageRepository.GetAllComments(secondPage.Id, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(1, comments.Count());
      }

      [Test]
      public async Task GetAllComments_ApprovedOnly()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));

         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         firstComment.IsApproved = true;
         await pageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);

         var secondComment = MakeComment();
         secondComment.IsApproved = false;
         await pageRepository.SaveComment(firstPage.Id, secondComment).ConfigureAwait(false);

         var comments = await pageRepository.GetAllComments(firstPage.Id, true, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(1, comments.Count());
      }

      [Test]
      public async Task GetAllComments_ForAllPages()
      {
         const int pageIndex = 0;
         const int pageSize = int.MaxValue;
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));

         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         await pageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);

         var secondPage = await MakePage(siteId).ConfigureAwait(false);
         var secondComment = MakeComment();
         await pageRepository.SaveComment(secondPage.Id, secondComment).ConfigureAwait(false);

         var comments = await pageRepository.GetAllComments(null, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(1, comments.Count(p => p.Author == firstComment.Author), $"Author is '{firstComment.Author}'");
         Assert.AreEqual(1, comments.Count(p => p.Author == secondComment.Author), $"Author is '{secondComment.Author}'");
      }

      [Test]
      public async Task GetAllComments_WithPaging()
      {
         const int pageSize = 5;
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         for(var i = 0; i < 10; i++)
         {
            var firstComment = MakeComment();
            await pageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);
         }
         var firstPageWithComments = await pageRepository.GetAllComments(firstPage.Id, false, 0, pageSize).ConfigureAwait(false);
         var secondPageWithComments = await pageRepository.GetAllComments(firstPage.Id, false, 1, pageSize).ConfigureAwait(false);
         Assert.AreEqual(5, firstPageWithComments.Count());
         Assert.AreEqual(5, secondPageWithComments.Count());
         Assert.AreEqual(0, firstPageWithComments.Intersect(secondPageWithComments, new CommentComparer()).Count());
      }

      [Test]
      public async Task GetCommentById()
      {
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         await pageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);
         var comment = await pageRepository.GetCommentById(firstComment.Id).ConfigureAwait(false);
         Assert.AreEqual(firstComment.Body, comment.Body);
         Assert.AreEqual(firstComment.Id, comment.Id);
      }

      [Test]
      public void GetCommentById_GuidEmpty()
      {
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => pageRepository.GetCommentById(Guid.Empty));
         Assert.AreEqual("Must not be Guid.Empty [Code 210110-1823] (Parameter 'commentId')", ex.Message);
      }

      [Test]
      public async Task GetCommentById_RandomId()
      {
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var comment = await pageRepository.GetCommentById(Guid.NewGuid()).ConfigureAwait(false);
         Assert.IsNull(comment);
      }

      [Test]
      public async Task DeleteComment()
      {
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         await pageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);
         await pageRepository.DeleteComment(firstComment.Id).ConfigureAwait(false);
         var comment = await pageRepository.GetCommentById(firstComment.Id).ConfigureAwait(false);
         Assert.IsNull(comment);
      }

      [Test]
      public async Task DeleteComment_RandomId()
      {
         var pageRepository = new PageRepository(SessionFactory, new ContentServiceFactory(_contentFactory));
         await pageRepository.DeleteComment(Guid.NewGuid()).ConfigureAwait(false);
         // no assertion
      }

      private class CommentComparer : IEqualityComparer<Comment>
      {
         public bool Equals(Comment x, Comment y)
         {
            return x.Id == y.Id;
         }

         public int GetHashCode([DisallowNull] Comment obj)
         {
            return obj.Id.GetHashCode();
         }
      }

      private static Comment MakeComment()
      {
         const int with10Words = 10;
         var firstName = Faker.Name.FirstName();
         var lastName = Faker.Name.LastName();
         return new Comment
         {
            Author = $"{firstName} {lastName}",
            Email = Faker.Internet.Email(firstName),
            Body = Faker.Lorem.Sentence(with10Words),
            IsApproved = true
         };
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
