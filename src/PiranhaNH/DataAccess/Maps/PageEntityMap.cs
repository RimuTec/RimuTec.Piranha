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
         Map(e => e.IsHidden);
         Map(e => e.LastModified);
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

         References(e => e.PageType);
         References(e => e.Site);
         References(e => e.Parent);
      }
   }
}
