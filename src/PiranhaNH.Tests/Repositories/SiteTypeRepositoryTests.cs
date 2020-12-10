using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NUnit.Framework;
using Piranha.Models;
using RimuTec.PiranhaNH.DataAccess;

namespace RimuTec.PiranhaNH.Repositories
{
   [TestFixture]
   public class SiteTypeRepositoryTests
   {
      public SiteTypeRepositoryTests()
      {
         SessionFactory = Database.CreateSessionFactory();
      }

      [Test]
      public async Task GetAll()
      {
         var repository = new SiteTypeRepository(SessionFactory);
         var siteTypeId = $"MyFirstType{Guid.NewGuid():n}";
         var siteTypeModel = MakeSiteType(siteTypeId);
         await repository.Save(siteTypeModel).ConfigureAwait(false);
         var siteTypes = await repository.GetAll().ConfigureAwait(false);
         Assert.AreEqual(1, siteTypes.Count(x => x.Id == siteTypeId));
      }

      private static SiteType MakeSiteType(string siteTypeId) {
         return new SiteType {
                Id = siteTypeId,
                Regions = new List<RegionType>
                {
                    new RegionType
                    {
                        Id = "Body",
                        Fields = new List<FieldType>
                        {
                            new FieldType {
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
