using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
   internal class PageTypeEntityMap : ClassMap<PageTypeEntity>
   {
      public PageTypeEntityMap()
      {
         Table("PageType");

         Id(e => e.Id);
         Map(e => e.Created);
         Map(e => e.LastModified);

         Map(e => e.CLRType);
         Map(e => e.Body);
      }
   }
}
