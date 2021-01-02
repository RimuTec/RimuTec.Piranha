using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
   internal class ParamEntityMap : ClassMap<ParamEntity>
   {
      public ParamEntityMap()
      {
         Table("Param");

         Id(e => e.Id);

         Map(e => e.Created);
         Map(e => e.LastModified);

         Map(e => e.Description);
         Map(e => e.Key).Column("[Key]"); // Key is a reserved keyword in SQL Server. TODO: Need to test with other databases, e.g. Postgres
         Map(e => e.Value);
      }
   }
}
