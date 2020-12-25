using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(201225_1221)]
   public class M201225_1221_AddSiteFieldTable : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "SiteField";
         const string siteIdColumn = "SiteId";
         Create.Table(tableName)
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("CLRType").AsString(256).NotNullable()
            .WithColumn("FieldId").AsString(64).NotNullable()
            .WithColumn("RegionId").AsString(64).NotNullable()
            .WithColumn(siteIdColumn).AsGuid().NotNullable()
            .WithColumn("SortOrder").AsInt32().NotNullable()
            .WithColumn("Value").AsString(int.MaxValue).Nullable()
            ;

         Create.Index($"IDX_{tableName}_{siteIdColumn}")
            .OnTable(tableName)
            .OnColumn(siteIdColumn)
            ;
      }
   }
}
