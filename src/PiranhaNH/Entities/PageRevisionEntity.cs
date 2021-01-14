using System;

namespace RimuTec.PiranhaNH.Entities
{
   internal class PageRevisionEntity : ContentRevisionBaseEntity
   {
      //   /// <summary>
      //   /// Gets/sets the id of the page this revision
      //   /// belongs to.
      //   /// </summary>
      //   public Guid PageId { get; set; }

      public virtual double RevisionNumber { get; set; }

      /// <summary>
      /// Gets/sets the page this revision belongs to. If it's a draft for a new page, 
      /// this property may be null.
      /// </summary>
      public virtual PageEntity Page { get; set; }

      public virtual Guid SiteId { get; set; }
   }
}
