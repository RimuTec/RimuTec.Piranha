using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using Piranha;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Repositories;
using RimuTec.PiranhaNH.Entities;
using RimuTec.PiranhaNH.Services;

namespace RimuTec.PiranhaNH.Repositories
{
   internal class SiteRepository : RepositoryBase, ISiteRepository
   {
      public SiteRepository(ISessionFactory sessionFactory, IContentServiceFactory factory) : base(sessionFactory)
      {
         _contentService = factory.CreateSiteService();
      }

      public async Task Delete(Guid id)
      {
         await InTx(async session =>
         {
            var entity = await session.GetAsync<SiteEntity>(id).ConfigureAwait(false);
            if (entity != null)
            {
               await session.DeleteAsync(entity).ConfigureAwait(false);
            }
         }).ConfigureAwait(false);
      }

      public async Task<IEnumerable<Site>> GetAll()
      {
         return await InTx(async session =>
         {
            var sites = new List<Site>();
            var entities = await session.Query<SiteEntity>().ToListAsync().ConfigureAwait(false);
            sites.AddRange(entities.Select(e => new Site
            {
               Id = e.Id,
               SiteTypeId = e.SiteTypeId,
               Title = e.Title,
               InternalId = e.InternalId,
               Description = e.Description,
               Logo = e.LogoId ?? new ImageField(),
               Hostnames = e.Hostnames,
               IsDefault = e.IsDefault,
               Culture = e.Culture,
               ContentLastModified = e.ContentLastModified,
               Created = e.Created,
               LastModified = e.LastModified
            }));
            return sites;
         }).ConfigureAwait(false);
      }

      public async Task<Site> GetById(Guid id)
      {
         return await InTx(async session =>
         {
            var entity = await session.GetAsync<SiteEntity>(id).ConfigureAwait(false);
            if (entity != null)
            {
               return new Site
               {
                  Id = entity.Id,
                  SiteTypeId = entity.SiteTypeId,
                  Title = entity.Title,
                  InternalId = entity.InternalId,
                  Description = entity.Description,
                  Logo = entity.LogoId ?? new ImageField(),
                  Hostnames = entity.Hostnames,
                  IsDefault = entity.IsDefault,
                  Culture = entity.Culture,
                  ContentLastModified = entity.ContentLastModified,
                  Created = entity.Created,
                  LastModified = entity.LastModified
               };
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task<Site> GetByInternalId(string internalId)
      {
         return await InTx(async session =>
         {
            Site siteModel = null;
            var entities = await session.Query<SiteEntity>().ToListAsync().ConfigureAwait(false);
            var siteEntity = entities.First(s => s.InternalId == internalId);
            if (siteEntity != null)
            {
               siteModel = new Site
               {
                  Id = siteEntity.Id,
                  ContentLastModified = siteEntity.ContentLastModified,
                  Created = siteEntity.Created,
                  Culture = siteEntity.Culture,
                  Description = siteEntity.Description,
                  Hostnames = siteEntity.Hostnames,
                  InternalId = siteEntity.InternalId,
                  IsDefault = siteEntity.IsDefault,
                  LastModified = siteEntity.LastModified,
                  Logo = siteEntity.LogoId,
                  SiteTypeId = siteEntity.SiteTypeId,
                  Title = siteEntity.Title
               };
            }
            return siteModel;
         }).ConfigureAwait(false);
      }

      public Task<DynamicSiteContent> GetContentById(Guid id)
      {
         return GetContentById<DynamicSiteContent>(id);
      }

      public async Task<T> GetContentById<T>(Guid id) where T : SiteContent<T>
      {
         return await InTx(async session =>
         {
            var site = await session.GetAsync<SiteEntity>(id).ConfigureAwait(false);

            if (site == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(site.SiteTypeId))
            {
                return null;
            }

            var type = App.SiteTypes.GetById(site.SiteTypeId);
            if (type == null)
            {
                return null;
            }

            return await _contentService.TransformAsync<T>(site, type).ConfigureAwait(false);
         }).ConfigureAwait(false);
      }

      public async Task<Site> GetDefault()
      {
         return await InTx(async session =>
         {
            Site siteModel = null;
            var entities = await session.Query<SiteEntity>().ToListAsync().ConfigureAwait(false);
            var siteEntity = entities.Find(s => s.IsDefault);
            if (siteEntity != null)
            {
               siteModel = new Site
               {
                  Id = siteEntity.Id,
                  ContentLastModified = siteEntity.ContentLastModified,
                  Created = siteEntity.Created,
                  Culture = siteEntity.Culture,
                  Description = siteEntity.Description,
                  Hostnames = siteEntity.Hostnames,
                  InternalId = siteEntity.InternalId,
                  IsDefault = siteEntity.IsDefault,
                  LastModified = siteEntity.LastModified,
                  Logo = siteEntity.LogoId,
                  SiteTypeId = siteEntity.SiteTypeId,
                  Title = siteEntity.Title
               };
            }
            return siteModel;
         }).ConfigureAwait(false);
      }

      public Task<Sitemap> GetSitemap(Guid id, bool onlyPublished = true)
      {
         throw new NotImplementedException();
      }

      public async Task Save(Site model)
      {
         await InTx(async session =>
         {
            var now = DateTime.Now;
            SiteEntity existing = await session.GetAsync<SiteEntity>(model.Id).ConfigureAwait(false);
            SiteEntity entity = existing ?? new SiteEntity() { Created = now };
            entity.ContentLastModified = model.ContentLastModified;
            // entity.Created = model.Created;
            entity.LastModified = now;// model.LastModified;
            entity.Culture = model.Culture ?? string.Empty;
            entity.Description = model.Description;
            entity.Hostnames = model.Hostnames ?? string.Empty;
            entity.InternalId = model.InternalId;
            entity.IsDefault = model.IsDefault;
            entity.LogoId = model.Logo?.Id;
            entity.SiteTypeId = model.SiteTypeId ?? string.Empty;
            entity.Title = model.Title;
            // TODO: deal with fields
            // entity.Fields = ???

            await session.SaveOrUpdateAsync(entity).ConfigureAwait(false);
            model.Id = entity.Id;
         }).ConfigureAwait(false);
      }

      public async Task SaveContent<T>(Guid siteId, T content) where T : SiteContent<T>
      {
         await InTx(async session =>
         {
            SiteEntity site = await session.GetAsync<SiteEntity>(siteId).ConfigureAwait(false);
            if (site != null)
            {
               if (string.IsNullOrEmpty(site.SiteTypeId))
               {
                  throw new MissingFieldException("Can't save content for a site that doesn't have a Site Type Id.");
               }

               var type = App.SiteTypes.GetById(site.SiteTypeId);
               if (type == null)
               {
                  throw new MissingFieldException("The specified Site Type is missing. Can't save content.");
               }

               content.Id = siteId;
               content.TypeId = site.SiteTypeId;
               content.Title = site.Title;

               _contentService.Transform(content, type, site);

               // Make sure foreign key is set for fields
               foreach(var field in site.Fields)
               {
                  field.Site = site;
                  if(field.Id == Guid.Empty)
                  {
                     await session.SaveAsync(field).ConfigureAwait(false);
                  }
               }
               await session.UpdateAsync(site).ConfigureAwait(false);
            }
         }).ConfigureAwait(false);
      }

      private readonly IContentService<SiteEntity, SiteFieldEntity, SiteContentBase> _contentService;
   }
}
