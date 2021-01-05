using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
   internal class BlockFieldEntityMap : ClassMap<BlockFieldEntity>
   {
      public BlockFieldEntityMap()
      {
         Table("BlockField");

         Id(e => e.Id);

         Map(e => e.CLRType);
         Map(e => e.FieldId);
         Map(e => e.SortOrder);
         Map(e => e.Value);

         References(e => e.Block).Column("BlockId");
      }
   }
}
