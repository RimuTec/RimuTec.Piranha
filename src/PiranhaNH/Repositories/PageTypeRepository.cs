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
   internal class PageTypeRepository : RepositoryBase, IPageTypeRepository
   {
      public PageTypeRepository(ISessionFactory sessionFactory) : base(sessionFactory)
      {
      }

      public Task Delete(string id)
      {
         throw new System.NotImplementedException();
      }

      public async Task<IEnumerable<PageType>> GetAll()
      {
         return await InTx(async (session) =>
         {
            var pageTypes = new List<PageType>();
            var entities = await session.Query<PageTypeEntity>().ToListAsync().ConfigureAwait(false);
            pageTypes.AddRange(entities.Select(entity => JsonConvert.DeserializeObject<PageType>(entity.Body)));
            return pageTypes;
         }).ConfigureAwait(false);
      }

      public Task<PageType> GetById(string id)
      {
         throw new System.NotImplementedException();
      }

      public async Task Save(PageType model)
      {
         await InTx(async session =>
         {
            DateTime now = DateTime.Now;
            var type = await session.GetAsync<PageTypeEntity>(model.Id).ConfigureAwait(false) ?? new PageTypeEntity { Created = now, Id = model.Id };
            type.CLRType = model.CLRType;
            type.Body = JsonConvert.SerializeObject(model);
            type.LastModified = now;
            await session.SaveOrUpdateAsync(type).ConfigureAwait(false);
            model.Id = type.Id;
         }).ConfigureAwait(false);
      }
   }
}
