using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using Piranha.Models;
using RimuTec.Faker;
using RimuTec.PiranhaNH.DataAccess;

namespace RimuTec.PiranhaNH.Repositories
{
   [TestFixture]
   public class PostTypeRepositoryTests : FixtureBase
   {
      public PostTypeRepositoryTests()
      {
         SessionFactory = Database.CreateSessionFactory();
      }

      [Test]
      public async Task SaveGetById()
      {
         var repository = new PostTypeRepository(SessionFactory);
         var postType = MakePostType();
         var postTypeId = postType.Id;
         await repository.Save(postType).ConfigureAwait(false);
         var retrieved = await repository.GetById(postTypeId).ConfigureAwait(false);
         Assert.AreEqual(postTypeId, retrieved.Id);
         Assert.AreEqual(1, retrieved.Regions.Count);
      }

      private static PostType MakePostType()
      {
         return new PostType
         {
            Id = $"MyFirstType-{RandomNumber.Next()}",
            Regions = new List<RegionType>
                {
                    new RegionType
                    {
                        Id = "Body",
                        Fields = new List<FieldType>
                        {
                            new FieldType
                            {
                                Id = "Default",
                                Type = "Html"
                            }
                        }
                    }
                }
         };
      }

      private ISessionFactory SessionFactory { get; }
   }
}
