using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
   internal class BlockEntityMap : ClassMap<BlockEntity>
   {
      public BlockEntityMap()
      {
         Table("Block");

         Id(e => e.Id);
         Map(e => e.Created);
         Map(e => e.LastModified);

         Map(e => e.CLRType);
         Map(e => e.IsReusable);
         Map(e => e.Title);
         Map(e => e.ParentId);

         HasMany(e => e.Fields).KeyColumn("BlockId");
      }
   }
}
