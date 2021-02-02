using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Linq;
using Piranha;
using Piranha.Models;
using Piranha.Repositories;
using RimuTec.PiranhaNH.Entities;
using RimuTec.PiranhaNH.Services;

namespace RimuTec.PiranhaNH.Repositories
{
   internal class PageRepository : RepositoryBase, IPageRepository
   {
      public PageRepository(ISessionFactory sessionFactory, IContentServiceFactory factory, IMapper mapper) : base(sessionFactory)
      {
         _contentService = factory.CreatePageService();
         _mapper = mapper;
      }

      public async Task CreateRevision(Guid id, int revisions)
      {
         if(revisions < 0)
         {
            throw new ArgumentOutOfRangeException(nameof(revisions), "Must not be negative. [210131-1808]");
         }
         await InTx(async session => {
            var pageEntities = session.Query<PageEntity>().Where(p => p.Id == id);

            if (pageEntities.Count() > 0)
            {
               var page = _mapper.ProjectTo<PageEntity>(pageEntities).First();

               await session.SaveAsync(new PageRevisionEntity
               {
                  //Id = Guid.NewGuid(),
                  Page = page,
                  Data = JsonConvert.SerializeObject(page),
                  Created = page.LastModified
               }).ConfigureAwait(false);

               //await _db.SaveChangesAsync().ConfigureAwait(false);

               // Check if we have a limit set on the number of revisions
               // we want to store.
               if (revisions > 0)
               {
                  var existing = await session.Query<PageRevisionEntity>()
                     .Where(r => r.Page == page)
                     .OrderByDescending(r => r.Created)
                     .Select(r => r.Id)
                     .Take(revisions)
                     .ToListAsync()
                     .ConfigureAwait(false);

                  if (existing.Count == revisions)
                  {
                     var toDelete = await session.Query<PageRevisionEntity>()
                        .Where(r => r.Page == page && !existing.Contains(r.Id))
                        .ToListAsync()
                        .ConfigureAwait(false);

                     if (toDelete.Count > 0)
                     {
                        foreach(var item in toDelete)
                        {
                           await session.DeleteAsync(item).ConfigureAwait(false);
                        }
                        // _db.PageRevisions.RemoveRange(removed);
                        // await _db.SaveChangesAsync().ConfigureAwait(false);
                     }
                  }
               }
            }
         }).ConfigureAwait(false);
      }

      public async Task<IEnumerable<Guid>> Delete(Guid pageId)
      {
         return await InTx(async session =>
         {
            var affected = new List<Guid>();
            var pageEntity = await session.GetAsync<PageEntity>(pageId).ConfigureAwait(false);
            await session.DeleteAsync(pageEntity).ConfigureAwait(false);
            var siblings = await session.Query<PageEntity>().Where(p => p.Site == pageEntity.Site && p.Parent == pageEntity.Parent).ToListAsync().ConfigureAwait(false);
            affected.AddRange(MovePages(siblings, pageId, pageEntity.Site.Id, pageEntity.SortOrder + 1, false));
            return affected;
         }).ConfigureAwait(false);
      }

      public async Task DeleteComment(Guid id)
      {
         await InTx(async session =>
         {
            var comment = await session.GetAsync<PageCommentEntity>(id).ConfigureAwait(false);
            if (comment != null)
            {
               await session.DeleteAsync(comment).ConfigureAwait(false);
            }
         }).ConfigureAwait(false);
      }

      public async Task DeleteDraft(Guid pageId)
      {
         await InTx(async session =>
         {
            var page = await session.GetAsync<PageEntity>(pageId).ConfigureAwait(false);
            if(page != null)
            {
               var drafts = await session.Query<PageRevisionEntity>()
                  .Where(r => r.Page == page && r.RevisionNumber >= page.RevisionNumber)
                  .ToListAsync()
                  .ConfigureAwait(false)
                  ;
               foreach(var obj in new List<PageRevisionEntity>(drafts))
               {
                  await session.DeleteAsync(obj).ConfigureAwait(false);
               }
            }
         }).ConfigureAwait(false);
      }

      public async Task<IEnumerable<Guid>> GetAll(Guid siteId)
      {
         return await InTx(async session =>
         {
            var pageEntities = await session.Query<PageEntity>()
               .Where(p => p.Site.Id == siteId)
               .OrderBy(p => p.Parent.Id)
               .ThenBy(p => p.SortOrder)
               .ToListAsync()
               .ConfigureAwait(false);
            return pageEntities.Select(p => p.Id);
         }).ConfigureAwait(false);
      }

      public async Task<IEnumerable<Guid>> GetAllBlogs(Guid siteId)
      {
         return new List<Guid>();
      }

      public Task<IEnumerable<Comment>> GetAllComments(Guid? pageId, bool onlyApproved, int page, int pageSize)
      {
         return GetAllComments(pageId, onlyApproved, false, page, pageSize);
      }

      internal async Task<IEnumerable<Comment>> GetAllComments(Guid? pageId, bool onlyApproved, bool onlyPending, int page, int pageSize)
      {
         return await InTx(async session =>
         {
            var comments = new List<Comment>();

            // Create base query
            var query = session.Query<PageCommentEntity>();

            // Check if only should include a comments for a certain post
            if (pageId.HasValue)
            {
               query = query.Where(c => c.Page.Id == pageId);
            }

            // Check if we should only include approved
            if (onlyPending)
            {
                query = query.Where(c => !c.IsApproved);
            }
            else if (onlyApproved)
            {
                query = query.Where(c => c.IsApproved);
            }

            // Check if this is a paged query
            if (pageSize > 0)
            {
               query = query
                  .Skip(page * pageSize)
                  .Take(pageSize);
            }

            // Get the comments
            var entities = await query
               .ToListAsync()
               .ConfigureAwait(false);
            comments.AddRange(entities.Select(e =>
            {
               return new Comment
               {
                  Id = e.Id,
                  Created = e.Created,
                  Author = e.Author,
                  Email = e.Email,
                  Body = e.Body,
                  IsApproved = e.IsApproved
               };
            }));
            return comments;
         }).ConfigureAwait(false);
      }

      public Task<IEnumerable<Guid>> GetAllDrafts(Guid siteId)
      {
         throw new NotImplementedException();
      }

      public Task<IEnumerable<Comment>> GetAllPendingComments(Guid? pageId, int page, int pageSize)
      {
         return GetAllComments(pageId, false, true, page, pageSize);
      }

      public async Task<T> GetById<T>(Guid pageId) where T : PageBase
      {
         return await InTx(async (session) =>
         {
            var page = await session.GetAsync<PageEntity>(pageId).ConfigureAwait(false);
            if (page != null)
            {
               return await _contentService.TransformAsync<T>(page, App.PageTypes.GetById(page.PageType.Id), Process).ConfigureAwait(false);
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task<T> GetBySlug<T>(string slug, Guid siteId) where T : PageBase
      {
         return await InTx(async session =>
         {
            var page = await session.Query<PageEntity>()
                .FirstOrDefaultAsync(p => p.Site.Id == siteId && p.Slug == slug)
                .ConfigureAwait(false);
            if(page != null)
            {
               return await _contentService.TransformAsync<T>(page, App.PageTypes.GetById(page.PageType.Id), Process).ConfigureAwait(false);
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task<Comment> GetCommentById(Guid commentId)
      {
         if (commentId == Guid.Empty)
         {
            throw new ArgumentOutOfRangeException(nameof(commentId), "Must not be Guid.Empty [Code 210110-1823]");
         }
         return await InTx(async session =>
         {
            var comment = await session.GetAsync<PageCommentEntity>(commentId).ConfigureAwait(false);
            if (comment != null)
            {
               return new Comment
               {
                  Id = comment.Id,
                  ContentId = comment.Page.Id,
                  UserId = comment.UserId,
                  Author = comment.Author,
                  Email = comment.Email,
                  Url = comment.Url,
                  IsApproved = comment.IsApproved,
                  Body = comment.Body,
                  Created = comment.Created
               };
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task<T> GetDraftById<T>(Guid pageId) where T : PageBase
      {
         return await InTx(async session =>
         {
            var revisionNumber = await session.Query<PageEntity>()
                .Where(p => p.Id == pageId)
                .Select(p => p.RevisionNumber)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);
            var draft = await session.Query<PageRevisionEntity>()
                .FirstOrDefaultAsync(r => r.Page.Id == pageId && r.RevisionNumber >= revisionNumber)
                .ConfigureAwait(false);

            if (draft != null)
            {
               return JsonConvert.DeserializeObject<T>(draft.Data);
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task<T> GetStartpage<T>(Guid siteId) where T : PageBase
      {
         // var page = await GetQuery<T>()
         //     .FirstOrDefaultAsync(p => p.SiteId == siteId && p.ParentId == null && p.SortOrder == 0)
         //     .ConfigureAwait(false);
         return await InTx(async session =>
         {
            var page = await session
               .Query<PageEntity>()
               .FirstOrDefaultAsync(p => p.Site.Id == siteId && p.Parent == null && p.SortOrder == 0)
               .ConfigureAwait(false);
            if (page != null)
            {
               return await _contentService.TransformAsync<T>(page, App.PageTypes.GetById(page.PageType.Id), Process).ConfigureAwait(false);
            }
            return null;
         }).ConfigureAwait(false);
      }

      public Task<IEnumerable<Guid>> Move<T>(T model, Guid? parentId, int sortOrder) where T : PageBase
      {
         throw new NotImplementedException();
      }

      public Task<IEnumerable<Guid>> Save<T>(T model) where T : PageBase
      {
         return Save(model, false);
      }

      public async Task SaveComment(Guid pageId, Comment model)
      {
         await InTx(async session =>
         {
            PageEntity pageEntity = await session.GetAsync<PageEntity>(pageId).ConfigureAwait(false);
            var pageComment = new PageCommentEntity
            {
               Page = pageEntity,
               Author = model.Author,
               Email = model.Email,
               Body = model.Body,
               IsApproved = model.IsApproved
            };
            await session.SaveAsync(pageComment).ConfigureAwait(false);
            model.Id = pageComment.Id;
         }).ConfigureAwait(false);
      }

      public Task SaveDraft<T>(T model) where T : PageBase
      {
         return Save(model, true);
      }

      /// <summary>
      /// Saves the given page model
      /// </summary>
      /// <param name="model">The page model</param>
      /// <param name="isDraft">If the model should be saved as a draft</param>
      private async Task<IEnumerable<Guid>> Save<T>(T model, bool isDraft) where T : PageBase
      {
         return await InTx(async session =>
         {
            var backThen = new DateTime(2020, 1, 1, 0, 0, 0);
            var revisionNumber = (DateTime.Now - backThen).TotalMilliseconds;

            var type = App.PageTypes.GetById(model.TypeId);
            var affected = new List<Guid>();
            var isNew = false;
            var lastModified = DateTime.MinValue;

            if (type == null)
            {
               return affected;
            }
            var page = await session.GetAsync<PageEntity>(model.Id).ConfigureAwait(false);
            //  IQueryable<Page> pageQuery = _db.Pages;
            //  if (isDraft)
            //  {
            //      pageQuery = pageQuery.AsNoTracking();
            //  }

            //  var page = await pageQuery
            //      .Include(p => p.Permissions)
            //      .Include(p => p.Blocks).ThenInclude(b => b.Block).ThenInclude(b => b.Fields)
            //      .Include(p => p.Fields)
            //      .FirstOrDefaultAsync(p => p.Id == model.Id)
            //      .ConfigureAwait(false);

            if (page == null)
            {
               isNew = true;
            }
            else
            {
               lastModified = page.LastModified;
            }

            if (model.OriginalPageId.HasValue)
            {
               var originalPage = await session.Query<PageEntity>().FirstOrDefaultAsync(p => p.Id == model.OriginalPageId).ConfigureAwait(false);
               //var originalPage = (await _db.Pages.AsNoTracking().FirstOrDefaultAsync(p => p.Id == model.OriginalPageId).ConfigureAwait(false));
               var originalPageIsCopy = originalPage?.OriginalPageId.HasValue ?? false;
               if (originalPageIsCopy)
               {
                  throw new InvalidOperationException("Can not set copy of a copy");
               }

               var originalPageType = originalPage?.PageType.Id;
               if (originalPageType != model.TypeId)
               {
                  throw new InvalidOperationException("Copy can not have a different content type");
               }

               // Transform the model
               if (page == null)
               {
                  page = new PageEntity()
                  {
                     //Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                     Created = DateTime.Now,
                  };

                  if (!isDraft)
                  {
                     await session.SaveAsync(page).ConfigureAwait(false);
                     //await _db.Pages.AddAsync(page).ConfigureAwait(false);

                     // Make room for the new page
                     var dest = await session.Query<PageEntity>().Where(p => p.Site.Id == model.SiteId && p.Parent.Id == model.ParentId).ToListAsync().ConfigureAwait(false);
                     //var dest = await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId).ToListAsync().ConfigureAwait(false);
                     affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.SortOrder, true));
                  }
               }
               else
               {
                  // Check if the page has been moved
                  if (!isDraft && (page.Parent.Id != model.ParentId || page.SortOrder != model.SortOrder))
                  {
                     var source = await session.Query<PageEntity>().Where(p => p.Site == page.Site && p.Parent == page.Parent && p.Id != model.Id).ToListAsync().ConfigureAwait(false);
                     //var source = await _db.Pages.Where(p => p.SiteId == page.SiteId && p.ParentId == page.ParentId && p.Id != model.Id).ToListAsync().ConfigureAwait(false);
                     var dest = page.Parent.Id == model.ParentId ? source : await session.Query<PageEntity>().Where(p => p.Site.Id == model.SiteId && p.Parent.Id == model.ParentId).ToListAsync().ConfigureAwait(false);
                     //var dest = page.Parent.Id == model.ParentId ? source : await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId).ToListAsync().ConfigureAwait(false);

                     // Remove the old position for the page
                     affected.AddRange(MovePages(source, page.Id, page.Site.Id, page.SortOrder + 1, false));
                     // Add room for the new position of the page
                     affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.SortOrder, true));
                  }
               }

               if (!isDraft && (isNew || page.Title != model.Title || page.NavigationTitle != model.NavigationTitle))
               {
                  // If this is new page or title has been updated it means
                  // the global sitemap changes. Notify the service.
                  affected.Add(page.Id);
               }

               page.ContentType = type.IsArchive ? "Blog" : "Page";
               page.PageType = await session.GetAsync<PageTypeEntity>(model.TypeId).ConfigureAwait(false);
               //page.PageTypeId = model.TypeId;
               page.OriginalPageId = model.OriginalPageId;
               page.Site = await session.GetAsync<SiteEntity>(model.SiteId).ConfigureAwait(false);
               //page.SiteId = model.SiteId;
               page.Title = model.Title;
               page.NavigationTitle = model.NavigationTitle;
               page.Slug = model.Slug;
               page.Parent = await session.GetAsync<PageEntity>(model.ParentId).ConfigureAwait(false);
               //page.ParentId = model.ParentId;
               page.SortOrder = model.SortOrder;
               page.IsHidden = model.IsHidden;
               page.Route = model.Route;
               page.Published = model.Published;
               page.RevisionNumber = revisionNumber;
               //page.LastModified = DateTime.Now;

               page.Permissions.Clear();
               foreach (var permission in model.Permissions)
               {
                  page.Permissions.Add(new PagePermissionEntity
                  {
                     PageId = page.Id,
                     Permission = permission
                  });
               }

               if (!isDraft)
               {
                  // NHibernate tracks changes automatically and writes on commit
                  //await _db.SaveChangesAsync().ConfigureAwait(false);
               }
               else
               {
                  await MakeDraft(session, model, revisionNumber, lastModified, page).ConfigureAwait(false);
                  // model.Id = await MakeDraft(session, page, revisionNumber, lastModified).ConfigureAwait(false);
                  model.Id = page.Id;
               }
               return affected;
            }

            // Transform the model
            if (page == null)
            {
               page = new PageEntity
               {
                  //Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                  Parent = (model.ParentId != null && model.ParentId != Guid.Empty) ? await session.GetAsync<PageEntity>(model.ParentId).ConfigureAwait(false) : null,
                  //ParentId = model.ParentId,
                  SortOrder = model.SortOrder,
                  PageType = await session.GetAsync<PageTypeEntity>(model.TypeId).ConfigureAwait(false),
                  //PageTypeId = model.TypeId,
                  //Created = DateTime.Now,
                  RevisionNumber = revisionNumber,
                  //LastModified = DateTime.Now,
                  // ### Begin RT changes ###
                  Title = model.Title,
                  Site = await session.GetAsync<SiteEntity>(model.SiteId).ConfigureAwait(false)
                  // ### End RT changes #####
               };

               if (!isDraft)
               {
                  await session.SaveAsync(page).ConfigureAwait(false);
                  model.Id = page.Id; // Only after the new entity object has been saved to NHibernate
                  var dest = await session.Query<PageEntity>().Where(p => p.Site != null && p.Parent != null && p.Site.Id == model.SiteId && p.Parent.Id == model.ParentId).ToListAsync().ConfigureAwait(false);
                  //var dest = await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId).ToListAsync().ConfigureAwait(false);
                  affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.SortOrder, true));
               }
            }
            else
            {
               // ### Begin RT changes ###
               page.Parent = (model.ParentId != null && model.ParentId != Guid.Empty) ? await session.GetAsync<PageEntity>(model.ParentId).ConfigureAwait(false) : null;
               // ### End RT changes #####
               // Check if the page has been moved
               if (page.Parent != null && !isDraft && (page.Parent.Id != model.ParentId || page.SortOrder != model.SortOrder))
               {
                  // ### Begin RT changes ###
                  // var siteId = $"Site id = '{page.Site.Id}'";
                  // var isPageParentNull = $"Page parent is null '{page.Parent == null}'";
                  // var parentId = $"Parent id = '{page.Parent.Id}'";
                  // ### End RT changes #####
                  var source = await session.Query<PageEntity>().Where(p => p.Site == page.Site && p.Parent == page.Parent && p.Id != model.Id).ToListAsync().ConfigureAwait(false);
                  //var source = await _db.Pages.Where(p => p.SiteId == page.SiteId && p.ParentId == page.Parent.Id && p.Id != model.Id).ToListAsync().ConfigureAwait(false);
                  var dest = page.Parent.Id == model.ParentId ? source : await session.Query<PageEntity>().Where(p => p.Site.Id == model.SiteId && p.Parent != null && p.Parent.Id == model.ParentId).ToListAsync().ConfigureAwait(false);
                  //var dest = page.Parent.Id == model.ParentId ? source : await _db.Pages.Where(p => p.SiteId == model.SiteId && p.ParentId == model.ParentId).ToListAsync().ConfigureAwait(false);

                  // Remove the old position for the page
                  affected.AddRange(MovePages(source, page.Id, page.Site.Id, page.SortOrder + 1, false));
                  // Add room for the new position of the page
                  affected.AddRange(MovePages(dest, page.Id, model.SiteId, model.SortOrder, true));
               }
               page.LastModified = DateTime.Now;
            }

            if (isNew || page.Title != model.Title || page.NavigationTitle != model.NavigationTitle)
            {
               // If this is new page or title has been updated it means
               // the global sitemap changes. Notify the service.
               affected.Add(page.Id);
            }

            page = _contentService.Transform(model, type, page);
            page.ContentType = type.IsArchive ? "Blog" : "Page";

            // Set if comments should be enabled
            page.EnableComments = model.EnableComments;
            page.CloseCommentsAfterDays = model.CloseCommentsAfterDays;

            // Update permissions
            page.Permissions.Clear();
            foreach (var permission in model.Permissions)
            {
               page.Permissions.Add(new PagePermissionEntity
               {
                  PageId = page.Id,
                  Permission = permission
               });
            }

            // Make sure foreign key is set for fields
            if (!isDraft)
            {
               foreach (var field in page.Fields)
               {
                  if (field.Page == null)
                  //if (field.PageId == Guid.Empty)
                  {
                     field.Page = page;
                     //field.PageId = page.Id;
                     await session.SaveOrUpdateAsync(field).ConfigureAwait(false);
                     //await _db.PageFields.AddAsync(field).ConfigureAwait(false);
                  }
               }
            }

            // Transform blocks
            var blockModels = model.Blocks;

            if (blockModels != null)
            {
               var blocks = _contentService.TransformBlocks(blockModels, session);

               var current = blocks.Select(b => b.Id).ToArray();

               // Delete removed blocks
               var removed = page.Blocks
                   .Where(b => !current.Contains(b.Block.Id) && !b.Block.IsReusable && b.Block.ParentId == null)
                   .Select(b => b.Block);
               var removedItems = page.Blocks
                   .Where(b => !current.Contains(b.Block.Id) && b.Block.ParentId != null && removed.Select(p => p.Id).ToList().Contains(b.Block.ParentId.Value))
                   .Select(b => b.Block);

               if (!isDraft)
               {
                  foreach (var block in removed)
                  {
                     await session.DeleteAsync(block).ConfigureAwait(false);
                  }
                  //_db.Blocks.RemoveRange(removed);
                  foreach (var block in removedItems)
                  {
                     await session.DeleteAsync(block).ConfigureAwait(false);
                  }
                  //_db.Blocks.RemoveRange(removedItems);
               }

               // Delete the old page blocks
               page.Blocks.Clear();

               // Now map the new block
               for (var n = 0; n < blocks.Count; n++)
               {
                  var block = await session.GetAsync<BlockEntity>(blocks[n].Id).ConfigureAwait(false);
                  //var block = await session.Query<BlockEntity>().FirstOrDefaultAsync(b => b.Id == blocks[n].Id).ConfigureAwait(false);
                  // IQueryable<BlockEntity> blockQuery = _db.Blocks;
                  // if (isDraft)
                  // {
                  //     blockQuery = blockQuery.AsNoTracking();
                  // }

                  // var block = await blockQuery
                  //     .Include(b => b.Fields)
                  //     .FirstOrDefaultAsync(b => b.Id == blocks[n].Id)
                  //     .ConfigureAwait(false);

                  if (block == null)
                  {
                     block = new BlockEntity
                     {
                        //Id = blocks[n].Id != Guid.Empty ? blocks[n].Id : Guid.NewGuid(),
                        Created = DateTime.Now
                     };
                     if (!isDraft)
                     {
                        session.Save(block);
                     }
                  }
                  block.ParentId = blocks[n].ParentId;
                  block.CLRType = blocks[n].CLRType;
                  block.IsReusable = blocks[n].IsReusable;
                  block.Title = blocks[n].Title;
                  block.LastModified = DateTime.Now;

                  session.Flush();

                  var currentFields = blocks[n].Fields.Select(f => f.FieldId).Distinct();
                  var removedFields = block.Fields.Where(f => !currentFields.Contains(f.FieldId));

                  if (!isDraft)
                  {
                     foreach (var field in removedFields)
                     {
                        await session.DeleteAsync(field).ConfigureAwait(false);
                     }
                     //_db.BlockFields.RemoveRange(removedFields);
                  }

                  foreach (var newField in blocks[n].Fields)
                  {
                     var field = block.Fields.FirstOrDefault(f => f.FieldId == newField.FieldId);
                     if (field == null)
                     {
                        field = new BlockFieldEntity
                        {
                           //Id = newField.Id != Guid.Empty ? newField.Id : Guid.NewGuid(),
                           Block = block,
                           //BlockId = block.Id,
                           FieldId = newField.FieldId
                           //CLRType = newField.CLRType
                        };
                        if (!isDraft)
                        {
                           //await session.SaveAsync(field).ConfigureAwait(false);
                           //await _db.BlockFields.AddAsync(field).ConfigureAwait(false);
                        }
                        block.Fields.Add(field);
                     }
                     field.SortOrder = newField.SortOrder;
                     field.CLRType = newField.CLRType;
                     field.Value = newField.Value;
                     if (!isDraft)
                     {
                        await session.SaveOrUpdateAsync(field).ConfigureAwait(false);
                     }
                  }

                  // Create the page block
                  var pageBlock = new PageBlockEntity
                  {
                     //Id = Guid.NewGuid(),
                     Block = block,
                     //BlockId = block.Id,
                     Page = page,
                     //PageId = page.Id,
                     SortOrder = n
                  };
                  if (!isDraft)
                  {
                     await session.SaveAsync(pageBlock).ConfigureAwait(false);
                     //await _db.PageBlocks.AddAsync(pageBlock).ConfigureAwait(false);
                  }
                  page.Blocks.Add(pageBlock);
               }
            }
            if (!isDraft)
            {
               // NHibernate keeps track of changes and writes as needed.
               //await _db.SaveChangesAsync().ConfigureAwait(false);
            }
            else
            {
               await MakeDraft(session, model, revisionNumber, lastModified, page).ConfigureAwait(false);
               model.Id = page.Id;
               // model.Id = await MakeDraft(session, page, revisionNumber, lastModified).ConfigureAwait(false);
            }
            return affected;
         }).ConfigureAwait(false);
      }

      private async Task<Guid> MakeDraft<T>(ISession session, T model, double revisionNumber, DateTime lastModified, PageEntity page) where T : PageBase
      {
         var draft = await session.Query<PageRevisionEntity>()
               .FirstOrDefaultAsync(r => r.Page.Id == model.Id && r.Created > lastModified)
               .ConfigureAwait(false);
         if (draft == null)
         {
            draft = new PageRevisionEntity
            {
               SiteId = model.SiteId,
               Page = page
            };
            await session.SaveAsync(draft).ConfigureAwait(false);
         }
         draft.Data = JsonConvert.SerializeObject(model);
         draft.RevisionNumber = revisionNumber;
         return draft.Id;
      }

      // private async Task<Guid> MakeDraft(ISession session, PageEntity page, double revisionNumber, DateTime lastModified)
      // {
      //    var draft = await session.Query<PageRevisionEntity>()
      //          .FirstOrDefaultAsync(r => r.Page.Id == page.Id && r.Created > lastModified)
      //          .ConfigureAwait(false);

      //    string pageId = "n/a";
      //    pageId = page?.Id.ToString();
      //    if (draft == null)
      //    {
      //       draft = new PageRevisionEntity
      //       {
      //          SiteId = page.Site.Id
      //       };
      //       await session.SaveAsync(draft).ConfigureAwait(false);
      //    }

      //    //var unproxied = session.GetSessionImplementation().PersistenceContext.Unproxy(page);

      //    var mapped = _mapper.Map<PageEntity,PageEntity>(page);

      //    //var mappedPageType = _mapper.Map<PageTypeEntity>(page.PageType).GetType();

      //    draft.Data = JsonConvert.SerializeObject(mapped);
      //    // draft.Data = JsonConvert.SerializeObject(page);
      //    draft.RevisionNumber = revisionNumber;

      //    // NHibernate keeps track of changes and writes as needed.
      //    //await _db.SaveChangesAsync().ConfigureAwait(false);
      //    return draft.Id;
      // }

      /// <summary>
      /// Performs additional processing and loads related models.
      /// </summary>
      /// <param name="page">The source page</param>
      /// <param name="model">The targe model</param>
      private Task Process<T>(PageEntity page, T model) where T : PageBase
      {
         // Permissions
         foreach (var permission in page.Permissions)
         {
            model.Permissions.Add(permission.Permission);
         }

         // Comments
         model.EnableComments = page.EnableComments;
         if (model.EnableComments)
         {
            model.CommentCount = page.Comments.Count;
            ///await _db.PageComments.CountAsync(c => c.PageId == model.Id).ConfigureAwait(false);
            //model.CommentCount = await _db.PageComments.CountAsync(c => c.PageId == model.Id).ConfigureAwait(false);
         }
         model.CloseCommentsAfterDays = page.CloseCommentsAfterDays;

         // Blocks
         if (!(model is IContentInfo))
         {
            if (page.Blocks.Count > 0)
            {
               foreach (var pageBlock in page.Blocks.OrderBy(b => b.SortOrder))
               {
                  if (pageBlock.Block.ParentId.HasValue)
                  {
                     var parent = page.Blocks.FirstOrDefault(b => b.Block.Id == pageBlock.Block.ParentId.Value);
                     if (parent != null)
                     {
                        pageBlock.Block.ParentId = parent.Block.Id;
                     }
                  }
               }
               model.Blocks = _contentService.TransformBlocks(page.Blocks.OrderBy(b => b.SortOrder).Select(b => b.Block));
            }
         }

         return Task.CompletedTask;
      }

      /// <summary>
      /// Moves the pages around. This is done when a page is deleted or moved in the structure.
      /// </summary>
      /// <param name="pages">The pages</param>
      /// <param name="pageId">The id of the page that is moved</param>
      /// <param name="siteId">The site id</param>
      /// <param name="sortOrder">The sort order</param>
      /// <param name="increase">If sort order should be increase or decreased</param>
      private IEnumerable<Guid> MovePages(IList<PageEntity> pages, Guid pageId, Guid siteId, int sortOrder, bool increase)
      {
         var affected = pages.Where(p => p.SortOrder >= sortOrder).ToList();

         foreach (var page in affected)
         {
            page.SortOrder = increase ? page.SortOrder + 1 : page.SortOrder - 1;
         }
         return affected.ConvertAll(p => p.Id);
      }

      private readonly IContentService<PageEntity, PageFieldEntity, PageBase> _contentService;
      private readonly IMapper _mapper;
   }
}
