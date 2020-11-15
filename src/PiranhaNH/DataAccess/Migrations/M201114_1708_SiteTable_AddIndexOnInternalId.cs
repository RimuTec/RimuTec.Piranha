using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
    [Migration(201114_1708)]
    public class M201114_1708_SiteTable_AddIndexOnInternalId : UpOnlyMigration
    {
        public override void Up()
        {
            Create.Index("IDX_Site_InternalId")
                .OnTable("Site")
                .OnColumn("InternalId")
                ;
        }
    }
}
