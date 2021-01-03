using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
   internal class PageCommentEntityMap : ClassMap<PageCommentEntity>
   {
      public PageCommentEntityMap()
      {
         Table("PageComment");

         Id(e => e.Id);
         Map(e => e.Created);

         Map(e => e.Author);
         Map(e => e.Body);
         Map(e => e.Email);
         Map(e => e.IsApproved);
         Map(e => e.Url);
         Map(e => e.UserId);

         References(e => e.Page);
      }
   }
}
