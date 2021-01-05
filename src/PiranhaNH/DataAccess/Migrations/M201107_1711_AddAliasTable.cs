using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
    [Migration(201107_1711)]
    public class M201107_1711_AddAliasTable : UpOnlyMigration
    {
        public override void Up()
        {
            const string TableName = "Alias";
            const string SiteIdColumn = "SiteId";
            const string AliasUrlColumn = "AliasUrl";
            Create.Table(TableName)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Created").AsDateTime2().NotNullable()
                .WithColumn("LastModified").AsDateTime2().NotNullable()
                .WithColumn(AliasUrlColumn).AsString(256).NotNullable()
                .WithColumn("RedirectUrl").AsString(256).NotNullable()
                .WithColumn(SiteIdColumn).AsGuid().NotNullable()
                .WithColumn("Type").AsString().NotNullable()
                ;

            Create.Index($"IX_{TableName}_{SiteIdColumn}_{AliasUrlColumn}")
                .OnTable(TableName)
                .OnColumn(SiteIdColumn).Ascending()
                .OnColumn(AliasUrlColumn).Ascending()
                .WithOptions()
                .Unique()
                ;
        }
    }
}
