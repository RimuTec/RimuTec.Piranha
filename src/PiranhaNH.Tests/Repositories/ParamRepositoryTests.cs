using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using Piranha.Models;
using RimuTec.Faker;
using RimuTec.PiranhaNH.DataAccess;

namespace RimuTec.PiranhaNH.Repositories
{
   [TestFixture]
   public class ParamRepositoryTests
   {
      public ParamRepositoryTests()
      {
         SessionFactory = Database.CreateSessionFactory();
      }

      [Test]
      public async Task GetByKey()
      {
         int aNumber = RandomNumber.Next(9999);
         string key = $"Key-{aNumber}";
         string value = $"Value-{aNumber}";
         var repository = new ParamRepository(SessionFactory);
         var param = new Param
         {
            Key = key,
            Value = value
         };
         await repository.Save(param).ConfigureAwait(false);
         var retrieved = await repository.GetByKey(key).ConfigureAwait(false);
         Assert.AreEqual(value, retrieved.Value);
      }

      private ISessionFactory SessionFactory { get; }
   }
}
