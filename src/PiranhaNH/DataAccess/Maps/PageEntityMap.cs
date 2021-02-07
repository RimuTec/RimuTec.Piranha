using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
   internal class PageEntityMap : ClassMap<PageEntity>
   {
      public PageEntityMap()
      {
         Table("Page");

         Id(e => e.Id);
         Map(e => e.Created);
         Map(e => e.LastModified);

         Map(e => e.RevisionNumber);
         Map(e => e.IsHidden);
         Map(e => e.MetaDescription);
         Map(e => e.MetaKeywords);
         Map(e => e.NavigationTitle);
         Map(e => e.Published);
         Map(e => e.RedirectType);
         Map(e => e.RedirectUrl);
         Map(e => e.Route);
         Map(e => e.Slug);
         Map(e => e.SortOrder);
         Map(e => e.Title);
         Map(e => e.CloseCommentsAfterDays);
         Map(e => e.EnableComments);
         Map(e => e.ContentType);

         References(e => e.PageType);
         References(e => e.Site);
         References(e => e.Parent);

         HasMany(e => e.Comments).KeyColumn("PageId").Cascade.AllDeleteOrphan();
         HasMany(e => e.Fields).KeyColumn("PageId")
            .Cascade.AllDeleteOrphan() // fields are children of page
            .Inverse() // field takes care of relationship, i.e. setting PageId
            ;
         HasMany(e => e.Blocks).KeyColumn("PageId");
         // HasMany(e => e.Permissions);
      }
   }
}
