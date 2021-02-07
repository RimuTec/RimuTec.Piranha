using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210207_1755)]
   public class M210207_1755_AddContentTypeColumn : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "Page";
         Alter.Table(tableName)
            .AddColumn("ContentType").AsString(255).NotNullable().WithDefaultValue("Page")
            ;
      }
   }
}
