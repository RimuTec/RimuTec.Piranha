using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Repositories;
using RimuTec.Piranha.Data.NH.Entities;

namespace RimuTec.Piranha.Data.NH.Repositories
{
    public class SiteRepository : ISiteRepository
    {
        public SiteRepository(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        private ISessionFactory SessionFactory { get; }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Site>> GetAll()
        {
            var models = new List<Site>();
            using(var session = SessionFactory.OpenSession()) {
                using(var txn = session.BeginTransaction())
                {
                    var entities = await session.Query<SiteEntity>().ToListAsync().ConfigureAwait(false);
                    models.AddRange(entities.Select(e => new Site {
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
                    await txn.CommitAsync().ConfigureAwait(false);
                }
            }
            return models;
        }

        public Task<Site> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<Site> GetByInternalId(string internalId)
        {
            Site siteModel = null;

            using(var session = SessionFactory.OpenSession())
            {
                using(var txn = session.BeginTransaction())
                {
                    var entities = await session.Query<SiteEntity>().ToListAsync().ConfigureAwait(false);
                    var siteEntity = entities.First(s => s.InternalId == internalId);
                    if(siteEntity != null)
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
                    await txn.CommitAsync().ConfigureAwait(false);
                }
            }
            return siteModel;
        }

        public Task<DynamicSiteContent> GetContentById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetContentById<T>(Guid id) where T : SiteContent<T>
        {
            throw new NotImplementedException();
        }

        public Task<Site> GetDefault()
        {
            throw new NotImplementedException();
        }

        public Task<Sitemap> GetSitemap(Guid id, bool onlyPublished = true)
        {
            throw new NotImplementedException();
        }

        public async Task Save(Site model)
        {
            using (var session = SessionFactory.OpenSession())
            {
                try
                {
                    using (var txn = session.BeginTransaction())
                    {
                        try
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
                            await txn.CommitAsync().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            txn.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        public Task SaveContent<T>(Guid siteId, T content) where T : SiteContent<T>
        {
            throw new NotImplementedException();
        }
    }
}