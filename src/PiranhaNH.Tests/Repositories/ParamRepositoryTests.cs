using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
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
         
      }

      private ISessionFactory SessionFactory { get; }
   }
}
