using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210104_1458)]
   public class M210104_1458_AddBlockTable : UpOnlyMigration
   {
      public override void Up()
      {
         Create.Table("Block")
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("Created").AsDateTime().NotNullable()
            .WithColumn("LastModified").AsDateTime().NotNullable()
            .WithColumn("CLRType").AsString(256).NotNullable()
            .WithColumn("IsReusable").AsBoolean().NotNullable()
            .WithColumn("ParentId").AsGuid().Nullable()
            .WithColumn("Title").AsString(128).Nullable()
            ;
      }
   }
}
