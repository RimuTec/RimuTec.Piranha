using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(201230_1829)]
   public class M201230_1829_AddPageTable : UpOnlyMigration
   {
      public override void Up()
      {
         Create.Table("Page")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Created").AsDateTime2().NotNullable()
            .WithColumn("LastModified").AsDateTime2().NotNullable()
            .WithColumn("IsHidden").AsBoolean().NotNullable()
            .WithColumn("MetaDescription").AsString(256).Nullable()
            .WithColumn("MetaKeywords").AsString(128).Nullable()
            .WithColumn("NavigationTitle").AsString(128).Nullable()
            .WithColumn("PageTypeId").AsString(64).NotNullable()
            .WithColumn("ParentId").AsGuid().Nullable()
            .WithColumn("Published").AsDateTime2().Nullable()
            .WithColumn("RedirectType").AsString().NotNullable()
            .WithColumn("RedirectUrl").AsString(256).Nullable()
            .WithColumn("Route").AsString(256).Nullable()
            .WithColumn("SiteId").AsGuid().NotNullable()
            .WithColumn("Slug").AsString(128).Nullable()
            .WithColumn("SortOrder").AsInt32().NotNullable()
            .WithColumn("Title").AsString(128).NotNullable()
            ;

         Create.Index("IDX_Page_PageTypeId")
            .OnTable("Page")
            .OnColumn("PageTypeId")
            ;

         Create.Index("IDX_Page_ParentId")
            .OnTable("Page")
            .OnColumn("ParentId")
            ;

         Create.Index("IDX_Page_SiteId")
            .OnTable("Page")
            .OnColumn("SiteId")
            ;
      }
   }
}
