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
   internal class PostTypeRepository : RepositoryBase, IPostTypeRepository
   {
      public PostTypeRepository(ISessionFactory sessionFactory) : base(sessionFactory)
      {
      }

      public Task Delete(string id)
      {
         throw new System.NotImplementedException();
      }

      public async Task<IEnumerable<PostType>> GetAll()
      {
         return await InTx(async (session) =>
         {
            var pageTypes = new List<PostType>();
            var entities = await session.Query<PostTypeEntity>().ToListAsync().ConfigureAwait(false);
            pageTypes.AddRange(entities.Select(entity => JsonConvert.DeserializeObject<PostType>(entity.Body)));
            return pageTypes;
         }).ConfigureAwait(false);
      }

      public async Task<PostType> GetById(string id)
      {
         return await InTx(async session =>
         {
            var type = await session.GetAsync<PostTypeEntity>(id).ConfigureAwait(false);
            if(type != null)
            {
               return JsonConvert.DeserializeObject<PostType>(type.Body);
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task Save(PostType model)
      {
         await InTx(async session =>
         {
            var now = DateTime.Now;
            var type = await session.GetAsync<PostTypeEntity>(model.Id).ConfigureAwait(false) ?? new PostTypeEntity { Id = model.Id, Created = now };
            type.CLRType = model.CLRType;
            type.Body = JsonConvert.SerializeObject(model);
            type.LastModified = now;
            await session.SaveOrUpdateAsync(type).ConfigureAwait(false);
         }).ConfigureAwait(false);
      }
   }
}
