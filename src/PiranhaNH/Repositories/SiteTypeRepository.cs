using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NHibernate;
using NHibernate.Linq;
using Piranha.Models;
using Piranha.Repositories;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.Repositories
{
   public class SiteTypeRepository : RepositoryBase, ISiteTypeRepository
   {
      public SiteTypeRepository(ISessionFactory sessionFactory) : base(sessionFactory)
      {
      }

      public async Task Delete(string id)
      {
         throw new System.NotImplementedException();
      }

      public async Task<IEnumerable<SiteType>> GetAll()
      {
         return await InTx(async session =>
         {
            // var models = new List<SiteType>();
            // var types = await session.Query<SiteTypeEntity>().OrderBy(e => e.Id).ToListAsync().ConfigureAwait(false);

            //var foo = await session.Query<SiteTypeEntity>().OrderBy(e => e.Id).

            return await (from entity
                      in session.Query<SiteTypeEntity>().OrderBy(e => e.Id)
                      select JsonConvert.DeserializeObject<SiteType>(entity.Body))
                      .ToListAsync()
                      .ConfigureAwait(false)
                      ;

            // models.AddRange(types.Select(type => JsonConvert.DeserializeObject<SiteType>(type.Body)));
            // return models;
         }).ConfigureAwait(false);
      }

      public async Task<SiteType> GetById(string siteTypeId)
      {
         throw new System.NotImplementedException();
      }

      public async Task Save(SiteType model)
      {
         await InTx(async session =>
         {
            var type = await session.GetAsync<SiteTypeEntity>(model.Id).ConfigureAwait(false);
            if(type == null)
            {
               type = new SiteTypeEntity
               {
                  Id = model.Id,
                  Created = DateTime.Now
               };
            }
            type.CLRType = model.CLRType;
            type.Body = JsonConvert.SerializeObject(model);
            type.LastModified = DateTime.Now;
            await session.SaveOrUpdateAsync(type).ConfigureAwait(false);
         }).ConfigureAwait(false);
      }
   }
}
