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
         throw new NotImplementedException();
      }

      public async Task<IEnumerable<PageType>> GetAll()
      {
         return await InTx(async (session) =>
         {
            var pageTypes = new List<PageType>();
            var entities = await session.Query<PageTypeEntity>().ToListAsync().ConfigureAwait(false);
            pageTypes.AddRange(entities.Where(e => e.Body != null).Select(entity => JsonConvert.DeserializeObject<PageType>(entity.Body)));
            return pageTypes;
         }).ConfigureAwait(false);
      }

      public async Task<PageType> GetById(string id)
      {
         return await InTx(async (session) =>
         {
            var entity = await session.GetAsync<PageTypeEntity>(id).ConfigureAwait(false);
            if(entity != null)
            {
               return JsonConvert.DeserializeObject<PageType>(entity.Body);
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task Save(PageType model)
      {
         if(model == null)
         {
            throw new ArgumentNullException(nameof(model), "Cannot be null. [Code 210103-1659]");
         }
         if(string.IsNullOrWhiteSpace(model.Id))
         {
            throw new ArgumentOutOfRangeException($"{nameof(model)}.{nameof(model.Id)}", "Cannot be null or whitespace. [Code 210103-1645]");
         }
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
