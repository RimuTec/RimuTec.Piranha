using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
   internal class PageFieldEntityMap : ClassMap<PageFieldEntity>
   {
      public PageFieldEntityMap()
      {
         Table("PageField");

         Id(e => e.Id);

         Map(e => e.CLRType);
         Map(e => e.SortOrder);
         Map(e => e.Value);
         Map(e => e.FieldId);
         Map(e => e.RegionId);

         References(e => e.Page);
      }
   }
}
