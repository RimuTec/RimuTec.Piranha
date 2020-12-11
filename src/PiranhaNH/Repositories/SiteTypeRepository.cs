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
         await InTx(async session =>
         {
            var entity = await session.GetAsync<SiteTypeEntity>(id).ConfigureAwait(false);
            if(entity != null)
            {
               await session.DeleteAsync(entity).ConfigureAwait(false);
            }
         }).ConfigureAwait(false);
      }

      public async Task<IEnumerable<SiteType>> GetAll()
      {
         return await InTx(async session =>
         {
            return await (from entity
                      in session.Query<SiteTypeEntity>().OrderBy(e => e.Id)
                      select JsonConvert.DeserializeObject<SiteType>(entity.Body))
                      .ToListAsync()
                      .ConfigureAwait(false)
                      ;
         }).ConfigureAwait(false);
      }

      public async Task<SiteType> GetById(string siteTypeId)
      {
         return await InTx(async session =>
         {
            var type = await session.GetAsync<SiteTypeEntity>(siteTypeId).ConfigureAwait(false);
            if(type != null) {
               return JsonConvert.DeserializeObject<SiteType>(type.Body);
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task Save(SiteType model)
      {
         await InTx(async session =>
         {
            DateTime now = DateTime.Now;
            var type = await session.GetAsync<SiteTypeEntity>(model.Id).ConfigureAwait(false);
            if(type == null)
            {
               type = new SiteTypeEntity
               {
                  Id = model.Id,
                  Created = now
               };
            }
            type.CLRType = model.CLRType;
            type.Body = JsonConvert.SerializeObject(model);
            type.LastModified = now;
            await session.SaveOrUpdateAsync(type).ConfigureAwait(false);
         }).ConfigureAwait(false);
      }
   }
}
