using System;
using System.Collections.Generic;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal class PageEntity : RoutedContentBaseEntity<PageFieldEntity>
   {
      // /// <summary>
      // /// Gets/sets the page type id.
      // /// </summary>
      // public string PageTypeId { get; set; }

      // /// <summary>
      // /// Gets/sets the site id.
      // /// </summary>
      // public Guid SiteId { get; set; }

      // /// <summary>
      // /// Gets/sets the optional parent id. Used to
      // /// position the page in the sitemap.
      // /// </summary>
      // public Guid? ParentId { get; set; }

      /// <summary>
      /// Gets/sets the type of content this page
      /// contains.
      /// </summary>
      public virtual string ContentType { get; set; }

      /// <summary>
      /// Gets/sets the optional primary image id.
      /// </summary>
      public virtual Guid? PrimaryImageId { get; set; }

      /// <summary>
      /// Gets/sets the optional excerpt.
      /// </summary>
      public virtual string Excerpt { get; set; }

      /// <summary>
      /// Gets/sets the pages sort order in its
      /// hierarchical position.
      /// </summary>
      public virtual int SortOrder { get; set; }

      /// <summary>
      /// Gets/sets the optional navigation title.
      /// </summary>
      public virtual string NavigationTitle { get; set; }

      /// <summary>
      /// Gets/sets if the page should be visible
      /// in the navigation.
      /// </summary>
      public virtual bool IsHidden { get; set; }

      /// <summary>
      /// Gets/sets the optional redirect.
      /// </summary>
      /// <returns></returns>
      public virtual string RedirectUrl { get; set; }

      /// <summary>
      /// Gets/sets the redirect type.
      /// </summary>
      /// <returns></returns>
      public virtual Piranha.Models.RedirectType RedirectType { get; set; } = Piranha.Models.RedirectType.Temporary;

      /// <summary>
      /// Gets/sets if comments should be enabled.
      /// </summary>
      /// <value></value>
      public virtual bool EnableComments { get; set; }

      /// <summary>
      /// Gets/sets after how many days after publish date comments
      /// should be closed. A value of 0 means never.
      /// </summary>
      public virtual int CloseCommentsAfterDays { get; set; }

      /// <summary>
      /// Gets/sets the site.
      /// </summary>
      public virtual SiteEntity Site { get; set; }

      /// <summary>
      /// Gets/sets the associated page type.
      /// </summary>
      public virtual PageTypeEntity PageType { get; set; }

      /// <summary>
      /// Gets/sets the optional page.
      /// </summary>
      public virtual PageEntity Parent { get; set; }

      /// <summary>
      /// Gets/sets the available page blocks.
      /// </summary>
      public virtual IList<PageBlockEntity> Blocks { get; set; } = new List<PageBlockEntity>();

      /// <summary>
      /// Gets/sets the available permissions.
      /// </summary>
      public virtual IList<PagePermissionEntity> Permissions { get; set; } = new List<PagePermissionEntity>();

      public virtual IList<PageCommentEntity> Comments { get; set; } = new List<PageCommentEntity>();

      /// <summary>
      /// Gets/sets the optional page this page is a copy of
      /// </summary>
      public virtual Guid? OriginalPageId { get; set; }
   }
}
