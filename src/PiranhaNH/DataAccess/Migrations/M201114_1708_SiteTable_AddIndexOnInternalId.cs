using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(201114_1708)]
   public class M201114_1708_SiteTable_AddIndexOnInternalId : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "Site";
         const string internalIdColumn = "InternalId";
         Create.Index(MakeIndexName(tableName, internalIdColumn))
             .OnTable("Site")
             .OnColumn("InternalId")
             ;
      }
   }
}
