using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(201210_0951)]
   public class M201210_0951_AddSiteTypeTable : UpOnlyMigration
   {
      public override void Up()
      {
         Create.Table("SiteType")
            .WithColumn("Id").AsString(64).NotNullable().PrimaryKey()
            .WithColumn("Created").AsDateTime2().NotNullable()
            .WithColumn("LastModified").AsDateTime2().NotNullable()
            .WithColumn("Body").AsString(int.MaxValue).Nullable()
            .WithColumn("CLRType").AsString(256).Nullable()
            ;
      }
   }
}
