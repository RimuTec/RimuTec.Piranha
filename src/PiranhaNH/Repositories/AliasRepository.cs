using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using Piranha.Models;
using Piranha.Repositories;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.Repositories
{
   public class AliasRepository : RepositoryBase, IAliasRepository
   {
      public AliasRepository(ISessionFactory sessionFactory) : base(sessionFactory)
      {
      }

      public async Task Delete(Guid id)
      {
         await InTx(async session =>
         {
            var toDelete = await session.GetAsync<AliasEntity>(id).ConfigureAwait(false);
            await session.DeleteAsync(toDelete).ConfigureAwait(false);
         })
         .ConfigureAwait(false);
      }

      public async Task<IEnumerable<Alias>> GetAll(Guid siteId)
      {
         return await InTx(async session =>
         {
            var aliases = new List<Alias>();
            var entities = await session.Query<AliasEntity>()
                                          .Where(a => a.Site.Id == siteId)
                                          .ToListAsync()
                                          .ConfigureAwait(false);
            aliases.AddRange(entities.Select(entity => new Alias
            {
               Id = entity.Id,
               AliasUrl = entity.AliasUrl,
               Created = entity.Created,
               LastModified = entity.LastModified,
               RedirectUrl = entity.RedirectUrl,
               SiteId = entity.Site.Id,
               Type = entity.Type
            }));
            return aliases;
         }
         ).ConfigureAwait(false);
      }

      public async Task<Alias> GetByAliasUrl(string url, Guid siteId)
      {
         return await InTx(async session =>
         {
            var entities = await session.Query<AliasEntity>()
                                          .Where(a => a.Site.Id == siteId
                                          && a.AliasUrl == url)
                                          .ToListAsync()
                                          .ConfigureAwait(false);
            if (entities.Count > 0)
            {
               var entity = entities[0];
               return new Alias
               {
                  Id = entity.Id,
                  AliasUrl = entity.AliasUrl,
                  Created = entity.Created,
                  LastModified = entity.LastModified,
                  RedirectUrl = entity.RedirectUrl,
                  SiteId = entity.Site.Id,
                  Type = entity.Type
               };
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task<Alias> GetById(Guid id)
      {
         return await InTx(async session =>
         {
            var entity = await session.GetAsync<AliasEntity>(id).ConfigureAwait(false);
            return new Alias
            {
               Id = entity.Id,
               AliasUrl = entity.AliasUrl,
               Created = entity.Created,
               LastModified = entity.LastModified,
               RedirectUrl = entity.RedirectUrl,
               SiteId = entity.Site.Id,
               Type = entity.Type
            };
         }).ConfigureAwait(false);
      }

      public async Task<IEnumerable<Alias>> GetByRedirectUrl(string url, Guid siteId)
      {
         return await InTx(async session =>
         {
            var aliases = new List<Alias>();
            var entities = await session.Query<AliasEntity>()
            .Where(a => a.RedirectUrl == url && a.Site.Id == siteId)
            .ToListAsync()
            .ConfigureAwait(false);
            aliases.AddRange(entities.Select(entity => new Alias
            {
               Id = entity.Id,
               AliasUrl = entity.AliasUrl,
               Created = entity.Created,
               LastModified = entity.LastModified,
               RedirectUrl = entity.RedirectUrl,
               SiteId = entity.Site.Id,
               Type = entity.Type
            }));
            return aliases;
         }).ConfigureAwait(false);
      }

      public async Task Save(Alias model)
      {
         await InTx(async session =>
         {
            AliasEntity entity = await session.GetAsync<AliasEntity>(model.Id).ConfigureAwait(false) ?? new AliasEntity();
            entity.Site = await session.GetAsync<SiteEntity>(model.SiteId).ConfigureAwait(false);
            entity.AliasUrl = model.AliasUrl;
            entity.RedirectUrl = model.RedirectUrl;
            entity.Type = model.Type;
            await session.SaveOrUpdateAsync(entity).ConfigureAwait(false);
            model.Id = entity.Id;
         }).ConfigureAwait(false);
      }
   }
}
