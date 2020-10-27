using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using Piranha.Models;
using Piranha.Repositories;

namespace RimuTec.Piranha.Repositories
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

        public Task<IEnumerable<Alias>> GetAll(Guid siteId)
        {
            throw new NotImplementedException();
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

        public Task Save(Alias model)
        {
            throw new NotImplementedException();
        }
    }
}
