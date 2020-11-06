using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
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
                    var entities = session.Query<SiteEntity>();
                    models.AddRange(entities.Select(e => new Site {
                        Id = e.Id,
                        SiteTypeId = e.SiteTypeId,
                        Title = e.Title,
                        InternalId = e.InternalId,
                        Description = e.Description,
                        Logo = e.LogoId.HasValue ? e.LogoId.Value : new ImageField(),
                        Hostnames = e.Hostnames,
                        IsDefault = e.IsDefault,
                        Culture = e.Culture,
                        ContentLastModified = e.ContentLastModified,
                        Created = e.Created,
                        LastModified = e.LastModified
                    }));
                    txn.Commit();
                }
            }
            return models;
        }

        public Task<Site> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Site> GetByInternalId(string internalId)
        {
            throw new NotImplementedException();
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
                            SiteEntity entity = session.Get<SiteEntity>(model.Id) ?? new SiteEntity();
                            // TODO: set properties
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
