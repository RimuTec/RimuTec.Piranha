using System;
using System.ComponentModel.DataAnnotations;
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

      [Test]
      public async Task UpdateParamValue()
      {
         var repository = new ParamRepository(SessionFactory);
         var param = MakeParam();
         await repository.Save(param).ConfigureAwait(false);
         string modifiedValue = param.Value + "-modified";
         param.Value = modifiedValue;
         await repository.Save(param).ConfigureAwait(false);
         var retrieved = await repository.GetByKey(param.Key).ConfigureAwait(false);
         Assert.AreEqual(modifiedValue, retrieved.Value);
      }

      [Test]
      public async Task DuplicateKey_ThrowsValidationException()
      {
         var repository = new ParamRepository(SessionFactory);
         var param11 = MakeParam();
         var param22 = MakeParam();
         param22.Key = param11.Key;
         await repository.Save(param11).ConfigureAwait(false);
         var ex = Assert.ThrowsAsync<ValidationException>(() => repository.Save(param22));
         Assert.AreEqual("The Key field must be unique", ex.Message);
      }

      // [Test]
      // public async Task UpdateDescription()
      // {
      //    var description = Company.Bs();
      //    var repository = new ParamRepository(SessionFactory);
      //    var param = MakeParam();
      //    await repository.Save(param).ConfigureAwait(false);
      //    param.Description = description;
      //    var retrieved = await repository.GetByKey(param.Key).ConfigureAwait(false);
      //    Assert.AreNotEqual(description, retrieved.Description);
      // }

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
