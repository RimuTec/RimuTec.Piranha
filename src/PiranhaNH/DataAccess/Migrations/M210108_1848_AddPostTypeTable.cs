using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210108_1848)]
   public class M210108_1848_AddPostTypeTable : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "PostType";
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
