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

      public Task<PostType> GetById(string id)
      {
         throw new System.NotImplementedException();
      }

      public Task Save(PostType model)
      {
         throw new System.NotImplementedException();
      }
   }
}
