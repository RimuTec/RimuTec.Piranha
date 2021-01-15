using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210111_1702)]
   public class M210111_1702_AddPageRevisionTable : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "PageRevision";
         const string pageIdColumn = "PageId";
         const string siteIdColumn = "SiteId";
         Create.Table(tableName)
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("Created").AsDateTime2().NotNullable()
            .WithColumn("LastModified").AsDateTime2().NotNullable()
            .WithColumn(pageIdColumn).AsGuid().NotNullable()
            .WithColumn("Data").AsString(int.MaxValue).Nullable()
            .WithColumn(siteIdColumn).AsGuid().NotNullable()
            ;

         Create.Index(MakeIndexName(tableName, pageIdColumn))
            .OnTable(tableName)
            .OnColumn(pageIdColumn)
            ;

         Create.Index(MakeIndexName(tableName, siteIdColumn))
            .OnTable(tableName)
            .OnColumn(siteIdColumn)
            ;
      }
   }
}
