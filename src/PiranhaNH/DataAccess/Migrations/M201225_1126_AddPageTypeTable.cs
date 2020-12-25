using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(201225_1126)]
   public class M201225_1126_AddPageTypeTable : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "PageType";
         Create.Table(tableName)
            .WithColumn("Id").AsString(64).NotNullable().PrimaryKey()
            .WithColumn("Created").AsDateTime().NotNullable()
            .WithColumn("LastModified").AsDateTime().NotNullable()
            .WithColumn("Body").AsString(int.MaxValue).Nullable()
            .WithColumn("CLRType").AsString(256).Nullable()
            ;
      }
   }
}
