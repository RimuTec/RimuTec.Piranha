using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
   internal class PageRevisionEntityMap : ClassMap<PageRevisionEntity>
   {
      public PageRevisionEntityMap()
      {
         Table("PageRevision");

         Id(e => e.Id);
         Map(e => e.Created);
         Map(e => e.LastModified);

         Map(e => e.RevisionNumber);
         Map(e => e.Data);
         Map(e => e.SiteId);

         References(e => e.Page);
      }
   }
}
