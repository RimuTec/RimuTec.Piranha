using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using Piranha.Models;
using Piranha.Repositories;
using RimuTec.Piranha.Data.NH.Entities;

namespace RimuTec.Piranha.Data.NH.Repositories
{
    public class AliasRepository : IAliasRepository
    {
        public AliasRepository(ISessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        private ISessionFactory SessionFactory { get; }

        public Task Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Alias>> GetAll(Guid siteId)
        {
            List<Alias> aliases = new List<Alias>();
            using (var session = SessionFactory.OpenSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    var entities = await session.Query<AliasEntity>()
                                                .Where(a => a.Site.Id == siteId)
                                                .ToListAsync()
                                                .ConfigureAwait(false);
                    aliases.AddRange(entities.Select(e => new Alias{
                        Id = e.Id,
                        AliasUrl = e.AliasUrl,
                        Created = e.Created,
                        LastModified = e.LastModified,
                        RedirectUrl = e.RedirectUrl,
                        SiteId = e.Site.Id,
                        Type = e.Type
                    }));
                    await txn.CommitAsync().ConfigureAwait(false);
                }
            }
            return aliases;
        }

        public Task<Alias> GetByAliasUrl(string url, Guid siteId)
        {
            throw new NotImplementedException();
        }

        public Task<Alias> GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Alias>> GetByRedirectUrl(string url, Guid siteId)
        {
            throw new NotImplementedException();
        }

        public async Task Save(Alias model)
        {
            using (var session = SessionFactory.OpenSession())
            {
                try
                {
                    using (var txn = session.BeginTransaction())
                    {
                        AliasEntity entity = await session.GetAsync<AliasEntity>(model.Id).ConfigureAwait(false) ?? new AliasEntity();
                        entity.Site = await session.GetAsync<SiteEntity>(model.SiteId).ConfigureAwait(false);
                        entity.AliasUrl = model.AliasUrl;
                        entity.RedirectUrl = model.AliasUrl;
                        entity.Type = model.Type;
                        await session.SaveOrUpdateAsync(entity).ConfigureAwait(false);
                        await txn.CommitAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}
