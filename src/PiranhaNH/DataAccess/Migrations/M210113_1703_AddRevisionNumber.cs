using System;
using FluentMigrator;

namespace RimuTec.PiranhaNH.DataAccess.Migrations
{
   [Migration(210113_1703)]
   public class M210113_1703_AddRevisionNumber : UpOnlyMigration
   {
      public override void Up()
      {
         var backThen = new DateTime(2020,1,1,0,0,0);
         var revisionNumber = (DateTime.Now - backThen).TotalMilliseconds;
         Alter.Table("Page")
            .AddColumn("RevisionNumber").AsDouble().NotNullable().WithDefaultValue(revisionNumber)
            ;
         Alter.Table("PageRevision")
            .AddColumn("RevisionNumber").AsDouble().NotNullable().WithDefaultValue(revisionNumber)
            ;
      }
   }
}
