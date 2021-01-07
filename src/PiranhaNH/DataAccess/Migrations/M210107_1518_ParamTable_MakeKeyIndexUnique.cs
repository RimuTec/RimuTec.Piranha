using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210107_1518)]
   public class M210107_1518_ParamTable_MakeKeyIndexUnique : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "Param";
         const string keyColumn = "Key";

         Delete.Index(MakeIndexName(tableName, keyColumn))
            .OnTable(tableName)
            ;

         Create.Index(MakeIndexName(tableName, keyColumn))
           .OnTable(tableName)
           .OnColumn(keyColumn)
           .Unique()
           ;
      }
   }
}
