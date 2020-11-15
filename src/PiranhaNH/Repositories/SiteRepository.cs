using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Repositories;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.Repositories
{
    public class SiteRepository : RepositoryBase, ISiteRepository
    {
        public SiteRepository(ISessionFactory sessionFactory) : base(sessionFactory)
        {
        }

        public async Task Delete(Guid id)
        {
            await InTx(async session => {
                var entity = await session.GetAsync<SiteEntity>(id).ConfigureAwait(false);
                if(entity != null) {
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
            return await InTx(async session => {
                var entity = await session.GetAsync<SiteEntity>(id).ConfigureAwait(false);
                if(entity != null) {
                    return new Site {
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
            throw new NotImplementedException();
        }

        public Task<T> GetContentById<T>(Guid id) where T : SiteContent<T>
        {
            throw new NotImplementedException();
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
                SiteEntity entity = await session.GetAsync<SiteEntity>(model.Id).ConfigureAwait(false) ?? new SiteEntity();
                entity.ContentLastModified = model.ContentLastModified;
                //entity.Created = model.Created;
                //entity.LastModified = model.LastModified;
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

        public Task SaveContent<T>(Guid siteId, T content) where T : SiteContent<T>
        {
            throw new NotImplementedException();
        }
    }
}
