using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Exceptions;
using NHibernate.Linq;
using Piranha.Models;
using Piranha.Repositories;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.Repositories
{
   internal class ParamRepository : RepositoryBase, IParamRepository
   {
      public ParamRepository(ISessionFactory sessionFactory) : base(sessionFactory)
      {
      }

      public async Task Delete(Guid id)
      {
         await InTx(async session =>
         {
            var entity = await session.GetAsync<ParamEntity>(id).ConfigureAwait(false);
            if (entity != null)
            {
               await session.DeleteAsync(entity).ConfigureAwait(false);
            }
         }).ConfigureAwait(false);
      }

      public Task<IEnumerable<Param>> GetAll()
      {
         throw new NotImplementedException();
      }

      public async Task<Param> GetById(Guid id)
      {
         return await InTx(async session =>
         {
            var param = await session.GetAsync<ParamEntity>(id).ConfigureAwait(false);
            if (param != null)
            {
               return new Param
               {
                  Id = param.Id,
                  Key = param.Key,
                  Description = param.Description,
                  Value = param.Value,
                  Created = param.Created,
                  LastModified = param.LastModified
               };
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task<Param> GetByKey(string key)
      {
         return await InTx(async session =>
         {
            var param = await session.Query<ParamEntity>().FirstOrDefaultAsync(p => p.Key == key).ConfigureAwait(false);
            if (param != null)
            {
               return new Param
               {
                  Id = param.Id,
                  Key = param.Key,
                  Description = param.Description,
                  Value = param.Value,
                  Created = param.Created,
                  LastModified = param.LastModified
               };
            }
            return null;
         }).ConfigureAwait(false);
      }

      public async Task Save(Param model)
      {
         try
         {
            await InTx(async session =>
            {
               ParamEntity entity = await session.GetAsync<ParamEntity>(model.Id).ConfigureAwait(false) ?? new ParamEntity();
               entity.Key = model.Key;
               entity.Value = model.Value;
               entity.Description = model.Description;
               model.Id = (Guid)await session.SaveAsync(entity).ConfigureAwait(false);
            }).ConfigureAwait(false);
         }
         catch (GenericADOException ex)
         {
            if (ex.IsDuplicateKey())
            {
               throw new ValidationException("The Key field must be unique");
            }
            TestContextWriter.WriteLine($"GenericADOException: HRESULT = {ex.HResult}");
            throw;
         }
      }
   }

   internal static class GenericADOExceptionExtensions
   {
      public static bool IsDuplicateKey(this GenericADOException exception)
      {
         return exception.InnerException is SqlException inner && inner.Number == 2601;
      }
   }

   internal static class TestContextWriter
   {
      static TestContextWriter()
      {
         var assemblies = AppDomain.CurrentDomain.GetAssemblies();
         var allTypes = new List<Type>();
         foreach (var assembly in assemblies)
         {
            allTypes.AddRange(assembly.GetTypes());
         }
         var testContext = allTypes.Single(t => t.FullName.Equals("nunit.framework.testcontext", StringComparison.OrdinalIgnoreCase));
         if (testContext != null)
         {
            _mi = testContext.GetMethod("WriteLine", new Type[] { typeof(string) });
         }
      }

      public static void WriteLine(string message)
      {
         _mi?.Invoke(null, new[] { message });
      }

      private static readonly MethodInfo _mi;
   }
}
