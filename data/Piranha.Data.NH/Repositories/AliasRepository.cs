using System;
using System.Collections.Generic;
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
                    aliases.AddRange(await session.Query<Alias>().ToListAsync().ConfigureAwait(false));
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
                        AliasEntity entity = session.Get<AliasEntity>(model.Id) ?? new AliasEntity();
                        entity.Site = session.Get<SiteEntity>(model.SiteId);
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
