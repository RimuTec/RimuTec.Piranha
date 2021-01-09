using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(211001_1156)]
   public class M210110_1156_PageComment_AddLastModified : UpOnlyMigration
   {
      public override void Up()
      {
         Alter.Table("PageComment")
            .AddColumn("LastModified").AsDateTime().NotNullable()
            ;
      }
   }
}
