using System;
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
         var repository = new ParamRepository(SessionFactory);
         Param param = MakeParam();
         await repository.Save(param).ConfigureAwait(false);
         var retrieved = await repository.GetByKey(param.Key).ConfigureAwait(false);
         Assert.AreEqual(param.Value, retrieved.Value);
      }

      [Test]
      public async Task Save_SetsIdOnModel()
      {
         var repository = new ParamRepository(SessionFactory);
         var param = MakeParam();
         await repository.Save(param).ConfigureAwait(false);
         Assert.AreNotEqual(Guid.Empty, param.Id);
      }

      private static Param MakeParam()
      {
         int aNumber = RandomNumber.Next(9999);
         string key = $"Key-{aNumber}";
         string value = $"Value-{aNumber}";
         return new Param
         {
            Key = key,
            Value = value
         };
      }

      private ISessionFactory SessionFactory { get; }
   }
}
