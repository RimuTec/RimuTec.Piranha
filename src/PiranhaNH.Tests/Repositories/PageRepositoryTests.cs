using NHibernate;
using RimuTec.PiranhaNH.DataAccess;

namespace RimuTec.PiranhaNH.Repositories
{
   public class PageRepositoryTests : FixtureBase
   {
      public PageRepositoryTests()
      {
         SessionFactory = Database.CreateSessionFactory();
      }

      private ISessionFactory SessionFactory { get; }
   }
}
