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
            await InTx(async session => {
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
                aliases.AddRange(entities.Select(e => new Alias
                {
                    Id = e.Id,
                    AliasUrl = e.AliasUrl,
                    Created = e.Created,
                    LastModified = e.LastModified,
                    RedirectUrl = e.RedirectUrl,
                    SiteId = e.Site.Id,
                    Type = e.Type
                }));
                return aliases;
            }
            ).ConfigureAwait(false);
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
            await InTx(async session => {
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
