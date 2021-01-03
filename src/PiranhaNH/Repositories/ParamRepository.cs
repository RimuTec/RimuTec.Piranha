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
   internal class ParamRepository : RepositoryBase, IParamRepository
   {
      public ParamRepository(ISessionFactory sessionFactory) : base(sessionFactory)
      {
      }

      public Task Delete(Guid id)
      {
         throw new NotImplementedException();
      }

      public Task<IEnumerable<Param>> GetAll()
      {
         throw new NotImplementedException();
      }

      public Task<Param> GetById(Guid id)
      {
         throw new NotImplementedException();
      }

      public async Task<Param> GetByKey(string key)
      {
         return await InTx(async session =>
         {
            var param = await session.Query<ParamEntity>().FirstOrDefaultAsync(p => p.Key == key).ConfigureAwait(false);
            if (param != null)
            {
               return new Param
               {
                  Id = param.Id,
                  Key = param.Key,
                  Description = param.Description,
                  Value = param.Value,
                  Created = param.Created,
                  LastModified = param.LastModified
               };
            }
            return null;
         }).ConfigureAwait(false);
      }

      public Task Save(Param model)
      {
         throw new NotImplementedException();
      }
   }
}