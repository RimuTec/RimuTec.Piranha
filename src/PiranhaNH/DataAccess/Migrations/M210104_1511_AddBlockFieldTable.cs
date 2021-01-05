using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210104_1511)]
   public class M210104_1511_AddBlockFieldTable : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "BlockField";
         const string blockIdColumn = "BlockId";
         Create.Table(tableName)
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn(blockIdColumn).AsGuid().NotNullable()
            .WithColumn("CLRType").AsString(256).NotNullable()
            .WithColumn("FieldId").AsString(64).NotNullable()
            .WithColumn("SortOrder").AsInt32().NotNullable()
            .WithColumn("Value").AsString(int.MaxValue).Nullable()
            ;
         Create.Index($"IDX_{tableName}_{blockIdColumn}")
            .OnTable(tableName)
            .OnColumn(blockIdColumn)
            ;
      }
   }
}
