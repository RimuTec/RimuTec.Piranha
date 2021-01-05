using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210104_1525)]
   public class M210104_1524_AddPageBlockTable : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "PageBlock";
         const string blockIdColumn = "BlockId";
         const string pageIdColumn = "PageId";
         Create.Table(tableName)
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn(blockIdColumn).AsGuid().NotNullable()
            .WithColumn(pageIdColumn).AsGuid().NotNullable()
            .WithColumn("SortOrder").AsInt32().NotNullable()
            .WithColumn("ParentId").AsGuid().Nullable()
            ;
         Create.Index($"IDX_{tableName}_{blockIdColumn}")
            .OnTable(tableName)
            .OnColumn(blockIdColumn)
            ;
         Create.Index($"IDX_{tableName}_{pageIdColumn}")
            .OnTable(tableName)
            .OnColumn(pageIdColumn)
            ;
      }
   }
}
