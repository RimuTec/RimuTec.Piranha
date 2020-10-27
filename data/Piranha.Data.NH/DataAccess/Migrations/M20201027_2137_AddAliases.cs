using FluentMigrator;

namespace RimuTec.Piranha.Data.NH.DataAccess.Migrations
{
    [Migration(20201027_2137)]
    public class M20201027_2137_AddAliases : UpOnlyMigration
    {
        public override void Up()
        {
            const string TableName = "Aliases";
            const string SiteIdColumn = "SiteId";
            const string AliasUrlColumn = "AliasUrl";
            Create.Table(TableName)
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Created").AsDateTimeOffset().NotNullable()
                .WithColumn("LastModified").AsDateTimeOffset().NotNullable()
                .WithColumn(AliasUrlColumn).AsString(256).NotNullable()
                .WithColumn("RedirectUrl").AsString(256).NotNullable()
                .WithColumn(SiteIdColumn).AsGuid().NotNullable()
                .WithColumn("Type").AsInt32().NotNullable()
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
