using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;
using Piranha.Models;
using Piranha.Repositories;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.Repositories
{
   internal class PageRepository : RepositoryBase, IPageRepository
   {
      public PageRepository(NHibernate.ISessionFactory sessionFactory) : base(sessionFactory)
      {
      }

      public Task CreateRevision(Guid id, int revisions)
      {
         throw new NotImplementedException();
      }

      public Task<IEnumerable<Guid>> Delete(Guid id)
      {
         throw new NotImplementedException();
      }

      public Task DeleteComment(Guid id)
      {
         throw new NotImplementedException();
      }

      public Task DeleteDraft(Guid id)
      {
         throw new NotImplementedException();
      }

      public async Task<IEnumerable<Guid>> GetAll(Guid siteId)
      {
         return await InTx(async session => {
            var pageEntities = await session.Query<PageEntity>()
               .Where(p => p.SiteId == siteId)
               .OrderBy(p => p.Parent.Id)
               .ThenBy(p => p.SortOrder)
               .ToListAsync()
               .ConfigureAwait(false);
            return pageEntities.Select(p => p.Id);
         }).ConfigureAwait(false);
      }

      public Task<IEnumerable<Guid>> GetAllBlogs(Guid siteId)
      {
         throw new NotImplementedException();
      }

      public Task<IEnumerable<Comment>> GetAllComments(Guid? pageId, bool onlyApproved, int page, int pageSize)
      {
         throw new NotImplementedException();
      }

      public Task<IEnumerable<Guid>> GetAllDrafts(Guid siteId)
      {
         throw new NotImplementedException();
      }

      public Task<IEnumerable<Comment>> GetAllPendingComments(Guid? pageId, int page, int pageSize)
      {
         throw new NotImplementedException();
      }

      public Task<T> GetById<T>(Guid id) where T : PageBase
      {
         throw new NotImplementedException();
      }

      public Task<T> GetBySlug<T>(string slug, Guid siteId) where T : PageBase
      {
         throw new NotImplementedException();
      }

      public Task<Comment> GetCommentById(Guid id)
      {
         throw new NotImplementedException();
      }

      public Task<T> GetDraftById<T>(Guid id) where T : PageBase
      {
         throw new NotImplementedException();
      }

      public Task<T> GetStartpage<T>(Guid siteId) where T : PageBase
      {
         throw new NotImplementedException();
      }

      public Task<IEnumerable<Guid>> Move<T>(T model, Guid? parentId, int sortOrder) where T : PageBase
      {
         throw new NotImplementedException();
      }

      public Task<IEnumerable<Guid>> Save<T>(T model) where T : PageBase
      {
         throw new NotImplementedException();
      }

      public Task SaveComment(Guid pageId, Comment model)
      {
         throw new NotImplementedException();
      }

      public Task SaveDraft<T>(T model) where T : PageBase
      {
         throw new NotImplementedException();
      }
   }
}
