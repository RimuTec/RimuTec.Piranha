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
      public async Task UpdateDescription()
      {
         var repository = new ParamRepository(SessionFactory);
         var param = MakeParam();
         await repository.Save(param).ConfigureAwait(false);
         var modifiedDescription = param.Description + " (modified)";
         param.Description = modifiedDescription;
         await repository.Save(param).ConfigureAwait(false);
         var retrieved = await repository.GetByKey(param.Key).ConfigureAwait(false);
         Assert.NotNull(param.Description);
         Assert.AreEqual(modifiedDescription, retrieved.Description);
      }

      [Test]
      public async Task GetById()
      {
         var repository = new ParamRepository(SessionFactory);
         var param = MakeParam();
         await repository.Save(param).ConfigureAwait(false);
         var paramId = param.Id;
         var retrieved = await repository.GetById(paramId).ConfigureAwait(false);
         Assert.AreEqual(paramId, retrieved.Id);
         Assert.AreEqual(param.Key, retrieved.Key);
         Assert.AreEqual(param.Value, retrieved.Value);
      }

      [Test]
      public async Task GetById_WithRandomId()
      {
         var repository = new ParamRepository(SessionFactory);
         var retrieved = await repository.GetById(Guid.NewGuid()).ConfigureAwait(false);
         Assert.Null(retrieved);
      }

      [Test]
      public async Task Delete()
      {
         var repository = new ParamRepository(SessionFactory);
         var param = MakeParam();
         await repository.Save(param).ConfigureAwait(false);
         var paramId = param.Id;
         await repository.Delete(paramId).ConfigureAwait(false);
         var retrieved = await repository.GetById(paramId).ConfigureAwait(false);
         Assert.Null(retrieved);
      }

      [Test]
      public async Task Delete_WithRandomId()
      {
         var repository = new ParamRepository(SessionFactory);
         await repository.Delete(Guid.NewGuid()).ConfigureAwait(false);
         // should not throw exception
      }

      private static Param MakeParam()
      {
         Guid aRandomValue = Guid.NewGuid();
         string key = $"Key-{aRandomValue:N}";
         string value = $"Value-{aRandomValue:N}";
         return new Param
         {
            Key = key,
            Value = value,
            Description = Company.Bs()
         };
      }

      private ISessionFactory SessionFactory { get; }
   }
}
