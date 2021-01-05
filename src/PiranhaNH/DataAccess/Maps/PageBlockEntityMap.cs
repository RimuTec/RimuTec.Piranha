using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
   internal class PageBlockEntityMap : ClassMap<PageBlockEntity>
   {
      public PageBlockEntityMap()
      {
         Table("PageBlock");

         Id(e => e.Id);
         Map(e => e.ParentId);
         Map(e => e.SortOrder);

         References(e => e.Block).Column("BlockId");
         References(e => e.Page).Column("PageId");
      }
   }
}
