using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210102_1446)]
   public class M210102_1446_AddPageFieldTable : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "PageField";
         const string pageIdColumn = "PageId";
         Create.Table(tableName)
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("CLRType").AsString(256).NotNullable()
            .WithColumn("FieldId").AsString(64).NotNullable()
            .WithColumn(pageIdColumn).AsGuid().NotNullable()
            .WithColumn("RegionId").AsString(64).NotNullable()
            .WithColumn("SortOrder").AsInt32().NotNullable()
            .WithColumn("Value").AsString(int.MaxValue).Nullable()
            ;

         Create.Index(MakeIndexName(tableName, pageIdColumn))
            .OnTable(tableName)
            .OnColumn(pageIdColumn)
            ;
      }
   }
}
