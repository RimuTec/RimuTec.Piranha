using FluentMigrator;

namespace RimuTec.Piranha.Data.NH.DataAccess.Migrations
{
    [Migration(201107_1712)]
    public class M201107_1712_AddSiteTable : UpOnlyMigration
    {
        public override void Up()
        {
            Create.Table("Site")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Created").AsDateTime2().NotNullable()
                .WithColumn("LastModified").AsDateTime2().NotNullable()
                .WithColumn("SiteTypeId").AsString().NotNullable()
                .WithColumn("InternalId").AsString().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("LogoId").AsGuid().Nullable()
                .WithColumn("Hostnames").AsString().NotNullable()
                .WithColumn("IsDefault").AsBoolean().NotNullable()
                .WithColumn("Culture").AsString().NotNullable()
                .WithColumn("ContentLastModified").AsDateTime2().Nullable()
                .WithColumn("Title").AsString().NotNullable()
                ;
        }
    }
}
