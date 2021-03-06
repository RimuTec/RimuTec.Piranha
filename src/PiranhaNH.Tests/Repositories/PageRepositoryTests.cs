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
using Piranha.Repositories;
using Piranha.Services;
using RimuTec.Faker;
using RimuTec.PiranhaNH.DataAccess;
using RimuTec.PiranhaNH.Entities;
using RimuTec.PiranhaNH.Services;

namespace RimuTec.PiranhaNH.Repositories
{
   [TestFixture]
   public class PageRepositoryTests : FixtureBase
   {
      public PageRepositoryTests()
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
      public async Task GetAll_ExistingSite()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var pageIds = await PageRepository.GetAll(siteId).ConfigureAwait(false);
         Assert.AreEqual(0, pageIds.Count());
      }

      [Test]
      public async Task GetAll_NonExistingSite()
      {
         var someRandomId = Guid.NewGuid();
         var pageIds = await PageRepository.GetAll(someRandomId).ConfigureAwait(false);
         Assert.AreEqual(0, pageIds.Count());
      }

      [Test]
      public async Task GetAll_WithOnePage()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         using var api = CreateApi();
         MyPage page = await MakePage(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         var pageIds = await PageRepository.GetAll(siteId).ConfigureAwait(false);
         Assert.AreEqual(1, pageIds.Count());
         Assert.AreEqual(1, pageIds.Count(id => id == pageId));
      }

      [Test]
      public async Task GetById()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         MyPage page = await MakePage(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         Assert.AreNotEqual(Guid.Empty, pageId);
         var retrieved = await PageRepository.GetById<MyPage>(pageId).ConfigureAwait(false);
         Assert.AreEqual(pageId, retrieved.Id);
         Assert.AreEqual(siteId, retrieved.SiteId);
         Assert.AreEqual("Startpage", retrieved.Title);
         Assert.AreEqual("Welcome", retrieved.Text.Value);
      }

      [Test]
      public async Task GetById_RandomIdReturnsNull()
      {
         var pageId = Guid.NewGuid();
         var retrieved = await PageRepository.GetById<MyPage>(pageId).ConfigureAwait(false);
         Assert.IsNull(retrieved);
      }

      [Test]
      public async Task GetById_PageWithBlocks()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         MyPage page = await MakePageWithBlocks(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         var retrieved = await PageRepository.GetById<MyPage>(pageId).ConfigureAwait(false);
         Assert.AreEqual(2, retrieved.Blocks.Count);
      }

      [Test]
      public async Task Save_AfterChange()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         MyPage page = await MakePage(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         string newText = $"Welcome at {DateTime.Now}";
         page.Text = newText;
         await PageRepository.Save(page).ConfigureAwait(false);
         var retrieved = await PageRepository.GetById<MyPage>(pageId).ConfigureAwait(false);
         Assert.AreEqual(newText, retrieved.Text.Value);
      }

      [Test]
      public async Task Delete()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         MyPage page = await MakePage(siteId).ConfigureAwait(false);
         var pageId = page.Id;
         await PageRepository.Delete(pageId).ConfigureAwait(false);
         var pageIds = await PageRepository.GetAll(siteId).ConfigureAwait(false);
         Assert.AreEqual(0, pageIds.Count());
         // TODO: assert that no orphan comments, blocks, fields, etc. are left.
      }

      [Test]
      public async Task GetStartPage_EmptySite()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var startPage = await PageRepository.GetStartpage<DynamicPage>(siteId).ConfigureAwait(false);
         Assert.IsNull(startPage);
      }

      [Test]
      public async Task GetStartPage_SinglePage()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var aPage = await MakePage(siteId).ConfigureAwait(false);
         aPage.ParentId = null;
         var pageId = aPage.Id;
         await PageRepository.Save(aPage).ConfigureAwait(false);
         var startPage = await PageRepository.GetStartpage<DynamicPage>(siteId).ConfigureAwait(false);
         Assert.AreEqual(pageId, startPage.Id);
      }

      [Test]
      public async Task GetStartPage_BasedOnParent()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var secondPage = await MakePage(siteId).ConfigureAwait(false);
         secondPage.ParentId = null;
         var secondPageId = secondPage.Id;
         firstPage.ParentId = secondPageId;
         await PageRepository.Save(secondPage).ConfigureAwait(false);
         await PageRepository.Save(firstPage).ConfigureAwait(false);
         var retrieved = await PageRepository.GetStartpage<DynamicPage>(siteId).ConfigureAwait(false);
         Assert.AreEqual(secondPageId, retrieved.Id);
      }

      [Test]
      public async Task GetStartPage_BasedOnSortOrder()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var secondPage = await MakePage(siteId).ConfigureAwait(false);
         var secondPageId = secondPage.Id;
         firstPage.SortOrder = 1;
         secondPage.SortOrder = 0;
         await PageRepository.Save(secondPage).ConfigureAwait(false);
         await PageRepository.Save(firstPage).ConfigureAwait(false);
         var retrieved = await PageRepository.GetStartpage<DynamicPage>(siteId).ConfigureAwait(false);
         Assert.AreEqual(secondPageId, retrieved.Id);
      }

      [Test]
      public async Task GetAllComments()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var comments = await PageRepository.GetAllComments(firstPage.Id, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(0, comments.Count());
      }

      [Test]
      public async Task GetAllComments_RandomPageId()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var randomPageId = Guid.NewGuid();
         var comments = await PageRepository.GetAllComments(randomPageId, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(0, comments.Count());
      }

      [Test]
      public async Task GetAllComments_PageWithOneComment()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var comment = MakeComment();
         await PageRepository.SaveComment(firstPage.Id, comment).ConfigureAwait(false);
         var comments = await PageRepository.GetAllComments(firstPage.Id, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(1, comments.Count());
      }

      [Test]
      public async Task GetAllComments_2PagesWithComment()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var siteId = await MakeSite().ConfigureAwait(false);

         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         await PageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);

         var secondPage = await MakePage(siteId).ConfigureAwait(false);
         var secondComment = MakeComment();
         await PageRepository.SaveComment(secondPage.Id, secondComment).ConfigureAwait(false);

         var comments = await PageRepository.GetAllComments(secondPage.Id, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(1, comments.Count());
      }

      [Test]
      public async Task GetAllComments_ApprovedOnly()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var siteId = await MakeSite().ConfigureAwait(false);

         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         firstComment.IsApproved = true;
         await PageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);

         var secondComment = MakeComment();
         secondComment.IsApproved = false;
         await PageRepository.SaveComment(firstPage.Id, secondComment).ConfigureAwait(false);

         var comments = await PageRepository.GetAllComments(firstPage.Id, true, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(1, comments.Count());
      }

      [Test]
      public async Task GetAllComments_ForAllPages()
      {
         const int pageIndex = 0;
         const int pageSize = int.MaxValue;
         var siteId = await MakeSite().ConfigureAwait(false);

         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         await PageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);

         var secondPage = await MakePage(siteId).ConfigureAwait(false);
         var secondComment = MakeComment();
         await PageRepository.SaveComment(secondPage.Id, secondComment).ConfigureAwait(false);

         var comments = await PageRepository.GetAllComments(null, false, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(1, comments.Count(p => p.Author == firstComment.Author), $"Author is '{firstComment.Author}'");
         Assert.AreEqual(1, comments.Count(p => p.Author == secondComment.Author), $"Author is '{secondComment.Author}'");
      }

      [Test]
      public async Task GetAllComments_WithPaging()
      {
         const int pageSize = 5;
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         for(var i = 0; i < 10; i++)
         {
            var firstComment = MakeComment();
            await PageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);
         }
         var firstPageWithComments = await PageRepository.GetAllComments(firstPage.Id, false, 0, pageSize).ConfigureAwait(false);
         var secondPageWithComments = await PageRepository.GetAllComments(firstPage.Id, false, 1, pageSize).ConfigureAwait(false);
         Assert.AreEqual(5, firstPageWithComments.Count());
         Assert.AreEqual(5, secondPageWithComments.Count());
         Assert.AreEqual(0, firstPageWithComments.Intersect(secondPageWithComments, new CommentComparer()).Count());
      }

      [Test]
      public async Task GetCommentById()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         await PageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);
         var comment = await PageRepository.GetCommentById(firstComment.Id).ConfigureAwait(false);
         Assert.AreEqual(firstComment.Body, comment.Body);
         Assert.AreEqual(firstComment.Id, comment.Id);
      }

      [Test]
      public void GetCommentById_GuidEmpty()
      {
         var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => PageRepository.GetCommentById(Guid.Empty));
         Assert.AreEqual("Must not be Guid.Empty [Code 210110-1823] (Parameter 'commentId')", ex.Message);
      }

      [Test]
      public async Task GetCommentById_RandomId()
      {
         var comment = await PageRepository.GetCommentById(Guid.NewGuid()).ConfigureAwait(false);
         Assert.IsNull(comment);
      }

      [Test]
      public async Task DeleteComment()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         await PageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);
         await PageRepository.DeleteComment(firstComment.Id).ConfigureAwait(false);
         var comment = await PageRepository.GetCommentById(firstComment.Id).ConfigureAwait(false);
         Assert.IsNull(comment);
      }

      [Test]
      public async Task DeleteComment_RandomId()
      {
         await PageRepository.DeleteComment(Guid.NewGuid()).ConfigureAwait(false);
         // no assertion
      }

      [Test]
      public async Task GetAllPendingComments()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         var firstComment = MakeComment();
         await PageRepository.SaveComment(firstPage.Id, firstComment).ConfigureAwait(false);
         var secondComment = MakeComment();
         secondComment.IsApproved = false;
         await PageRepository.SaveComment(firstPage.Id, secondComment).ConfigureAwait(false);
         var comments = await PageRepository.GetAllPendingComments(firstPage.Id, pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(1, comments.Count());
      }

      [Test]
      public async Task GetAllPendingComments_RandomId()
      {
         const int pageIndex = 0;
         const int pageSize = 10;
         var comments = await PageRepository.GetAllPendingComments(Guid.NewGuid(), pageIndex, pageSize).ConfigureAwait(false);
         Assert.AreEqual(0, comments.Count());
      }

      [Test]
      public async Task GetBySlug()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         firstPage.Slug = $"slug-{RandomNumber.Next()}";
         await PageRepository.Save(firstPage).ConfigureAwait(false);
         var retrieved = await PageRepository.GetBySlug<MyPage>(firstPage.Slug, siteId).ConfigureAwait(false);
         Assert.AreEqual(firstPage.Id, retrieved.Id);
      }

      [Test]
      public async Task GetBySlug_RandomSiteId()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         firstPage.Slug = $"slug-{RandomNumber.Next()}";
         await PageRepository.Save(firstPage).ConfigureAwait(false);
         var retrieved = await PageRepository.GetBySlug<MyPage>(firstPage.Slug, Guid.NewGuid()).ConfigureAwait(false);
         Assert.IsNull(retrieved);
      }

      [Test]
      public async Task GetBySlug_RandomSlug()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var firstPage = await MakePage(siteId).ConfigureAwait(false);
         firstPage.Slug = $"slug-{RandomNumber.Next()}";
         await PageRepository.Save(firstPage).ConfigureAwait(false);
         var anotherRandomSlug = $"slug-{RandomNumber.Next()}";
         var retrieved = await PageRepository.GetBySlug<MyPage>(anotherRandomSlug, siteId).ConfigureAwait(false);
         Assert.IsNull(retrieved);
      }

      [Test]
      public async Task Save_WithoutPublishDateShouldNotHaveDraft()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         using var api = CreateApi();
         MyPage page = await MyPage.CreateAsync(api).ConfigureAwait(false);
         page.SiteId = siteId;
         page.Title = "Startpage";
         page.Text = "Welcome";
         page.IsHidden = true;
         page.Published = null;
         await PageRepository.Save(page).ConfigureAwait(false);
         var pageId = page.Id;
         var draft = await PageRepository.GetDraftById<MyPage>(pageId).ConfigureAwait(false);
         Assert.IsNull(draft);
      }

      [Test]
      public async Task Save_WithPublishDateInFutureShouldNotHaveDraft()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         using var api = CreateApi();
         MyPage page = await MyPage.CreateAsync(api).ConfigureAwait(false);
         page.SiteId = siteId;
         page.Title = "Startpage";
         page.Text = "Welcome";
         page.IsHidden = true;
         page.Published = DateTime.Now.AddDays(42);
         await PageRepository.Save(page).ConfigureAwait(false);
         var pageId = page.Id;
         var draft = await PageRepository.GetDraftById<MyPage>(pageId).ConfigureAwait(false);
         Assert.IsNull(draft);
      }

      [Test]
      public async Task Save_WithPublishDateInPastShouldNotHaveDraft()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         using var api = CreateApi();
         MyPage page = await MyPage.CreateAsync(api).ConfigureAwait(false);
         page.SiteId = siteId;
         page.Title = "Startpage";
         page.Text = "Welcome";
         page.IsHidden = true;
         page.Published = DateTime.Now.AddDays(-42);
         await PageRepository.Save(page).ConfigureAwait(false);
         var pageId = page.Id;
         var draft = await PageRepository.GetDraftById<MyPage>(pageId).ConfigureAwait(false);
         Assert.IsNull(draft);
      }

      [Test]
      public async Task Save_UnpublishedTwiceDoesNotCreateDraft()
      {
         DateTime? unpublished = null;
         var siteId = await MakeSite().ConfigureAwait(false);
         using var api = CreateApi();
         MyPage page = await MyPage.CreateAsync(api).ConfigureAwait(false);
         page.SiteId = siteId;
         page.Title = "Startpage";
         page.Text = "Welcome";
         page.IsHidden = true;
         page.Published = unpublished;
         await PageRepository.Save(page).ConfigureAwait(false);
         page.Text = $"Welcome, ${Name.FirstName()}";
         await PageRepository.Save(page).ConfigureAwait(false);
         var pageId = page.Id;
         var draft = await PageRepository.GetDraftById<MyPage>(pageId).ConfigureAwait(false);
         Assert.IsNull(draft);
      }

      [Test]
      public async Task Save_PublishedAsDraft()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         using var api = CreateApi();
         MyPage page = await MyPage.CreateAsync(api).ConfigureAwait(false);
         page.SiteId = siteId;
         page.Title = "Startpage";
         page.Text = "Welcome";
         page.IsHidden = true;
         page.Published = DateTime.Now.AddDays(-1);
         await PageRepository.Save(page).ConfigureAwait(false);
         await PageRepository.SaveDraft(page).ConfigureAwait(false);
         var pageId = page.Id;
         var retrieved = await PageRepository.GetDraftById<MyPage>(pageId).ConfigureAwait(false);
         Assert.AreEqual(pageId, retrieved.Id);
         Assert.AreEqual(siteId, retrieved.SiteId);
      }

      [Test]
      public async Task DeleteDraft()
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
         await PageRepository.SaveDraft(page).ConfigureAwait(false);
         var draftId = page.Id;
         await PageRepository.DeleteDraft(draftId).ConfigureAwait(false);
         var retrieved = await PageRepository.GetDraftById<MyPage>(draftId).ConfigureAwait(false);
         Assert.IsNull(retrieved);
      }

      [Test]
      public async Task GetDraftById_RandomId()
      {
         var retrieved = await PageRepository.GetDraftById<MyPage>(Guid.NewGuid()).ConfigureAwait(false);
         Assert.IsNull(retrieved);
      }

      [Test]
      public async Task CreateRevision()
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
         await PageRepository.CreateRevision(pageId, 10).ConfigureAwait(false);
         using var session = SessionFactory.OpenSession();
         using var txn = session.BeginTransaction();
         var revisions = session.Query<PageRevisionEntity>().Where(p => p.Page.Id == pageId).ToList();
         Assert.AreEqual(1, revisions.Count);
         txn.Commit();
      }

      [Test]
      public void CreateRevision_RejectNegativeNumber()
      {
         var ex = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => PageRepository.CreateRevision(Guid.NewGuid(), -1));
         Assert.AreEqual("Must not be negative. [210131-1808] (Parameter 'revisions')", ex.Message);
      }

      [Test]
      public async Task CreateRevisions_WithRemovalOfOld()
      {
         const int revisionCount = 2;
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
         await PageRepository.CreateRevision(pageId, revisionCount).ConfigureAwait(false);
         await PageRepository.CreateRevision(pageId, revisionCount).ConfigureAwait(false);
         await PageRepository.CreateRevision(pageId, revisionCount).ConfigureAwait(false);
         using var session = SessionFactory.OpenSession();
         using var txn = session.BeginTransaction();
         var revisions = session.Query<PageRevisionEntity>().Where(p => p.Page.Id == pageId).ToList();
         Assert.AreEqual(revisionCount, revisions.Count);
         txn.Commit();
      }

      [Test]
      public async Task GetAllBlogs()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var blogIds = await PageRepository.GetAllBlogs(siteId).ConfigureAwait(false);
         Assert.IsEmpty(blogIds);
      }

      [Test]
      public async Task GetAllBlogs_WithOneBlog()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var blog = await MakeBlog(siteId).ConfigureAwait(false);
         var blogIds = await PageRepository.GetAllBlogs(siteId).ConfigureAwait(false);
         Assert.AreEqual(1, blogIds.Count());
         Assert.Contains(blog.Id, blogIds.ToList());
      }

      [Test]
      public async Task GetAllBlogs_SortOrder()
      {
         var siteId = await MakeSite().ConfigureAwait(false);
         var blog1 = await MakeBlog(siteId, 1).ConfigureAwait(false);
         var blog2 = await MakeBlog(siteId, 3).ConfigureAwait(false);
         var blog3 = await MakeBlog(siteId, 2).ConfigureAwait(false);
         var blogIds = await PageRepository.GetAllBlogs(siteId).ConfigureAwait(false);
         var array = blogIds.ToArray();
         Assert.AreEqual(3, array.Length);
         Assert.AreEqual(blog1.Id, array[0]);
         Assert.AreEqual(blog3.Id, array[1]);
         Assert.AreEqual(blog2.Id, array[2]);
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
         var firstName = Name.FirstName();
         var lastName = Name.LastName();
         return new Comment
         {
            Author = $"{firstName} {lastName}",
            Email = Internet.Email(firstName),
            Body = Lorem.Sentence(with10Words),
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
         using var api = CreateApi();
         MyPage page = await MyPage.CreateAsync(api).ConfigureAwait(false);
         page.SiteId = siteId;
         page.Title = "Startpage";
         page.Text = "Welcome";
         page.IsHidden = true;
         page.Published = DateTime.Now;
         await PageRepository.Save(page).ConfigureAwait(false);
         return page;
      }

      private async Task<MyBlogPage> MakeBlog(Guid siteId, int sortOrder = 0)
      {
         using var api = CreateApi();
         MyBlogPage blog = await MyBlogPage.CreateAsync(api).ConfigureAwait(false);
         blog.SiteId = siteId;
         blog.Title = "Blog Archive";
         blog.SortOrder = sortOrder;
         await PageRepository.Save(blog).ConfigureAwait(false);
         return blog;
      }

      private async Task<MyPage> MakePageWithBlocks(Guid siteId)
      {
         var page = await MakePage(siteId).ConfigureAwait(false);
         page.Blocks.Add(new Piranha.Extend.Blocks.TextBlock
         {
            Body = Lorem.Sentence(3)
         });
         page.Blocks.Add(new Piranha.Extend.Blocks.TextBlock
         {
            Body = Lorem.Sentence(3)
         });
         await PageRepository.Save(page).ConfigureAwait(false);
         return page;
      }

      private void Initialize()
      {
         using var api = CreateApi();
         Piranha.App.Init(api);

         var builder = new PageTypeBuilder(api)
            .AddType(typeof(MyPage))
            .AddType(typeof(MyBlogPage))
            ;
         builder.Build();

         var siteBuilder = new SiteTypeBuilder(api)
            .AddType(typeof(MySiteContent))
            ;
         siteBuilder.Build();
      }

      [PageType(Title = "PageType")]
      public class MyPage : Page<MyPage>
      {
         [Region]
         public TextField Text { get; set; }
      }

      [PageType(Title = "My BlogType", IsArchive = true)]
        public class MyBlogPage : Page<MyBlogPage>
        {
            [Region]
            public TextField Ingress { get; set; }
            [Region]
            public MarkdownField Body { get; set; }
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
