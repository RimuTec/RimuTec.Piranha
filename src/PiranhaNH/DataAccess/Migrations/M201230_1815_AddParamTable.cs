using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(201230_1815)]
   public class M201230_1815_AddParamTable : UpOnlyMigration
   {
      public override void Up()
      {
         const string keyColumn = "Key";

         Create.Table("Param")
            .WithColumn("Id").AsGuid().PrimaryKey().NotNullable()
            .WithColumn("Created").AsDateTime().NotNullable()
            .WithColumn("LastModified").AsDateTime().NotNullable()
            .WithColumn("Description").AsString(256).Nullable()
            .WithColumn(keyColumn).AsString(64).NotNullable()
            .WithColumn("Value").AsString(int.MaxValue).Nullable()
            ;

         Create.Index("IDX_Param_Key")
            .OnTable("Param")
            .OnColumn(keyColumn)
            ;
      }
   }
}
