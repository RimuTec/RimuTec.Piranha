using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210103_1831)]
   public class M210103_1831_AddPageCommentTable : UpOnlyMigration
   {
      public override void Up()
      {
         const string tableName = "PageComment";
         const string pageIdColumn = "PageId";
         Create.Table(tableName)
            .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("Created").AsDateTime2().NotNullable()
            .WithColumn("UserId").AsString().Nullable()
            .WithColumn("Author").AsString(128).NotNullable()
            .WithColumn("Email").AsString(128).NotNullable()
            .WithColumn("Url").AsString(256).Nullable()
            .WithColumn("IsApproved").AsBoolean().NotNullable()
            .WithColumn("Body").AsString(int.MaxValue).Nullable()
            .WithColumn(pageIdColumn).AsGuid().NotNullable()
            ;

         Create.Index(MakeIndexName(tableName, pageIdColumn))
            .OnTable(tableName)
            .OnColumn(pageIdColumn)
            ;

         Alter.Table("Page")
            .AddColumn("CloseCommentsAfterDays").AsInt32().NotNullable().WithDefaultValue(0)
            .AddColumn("EnableComments").AsBoolean().NotNullable().WithDefaultValue(false)
            ;
      }
   }
}
