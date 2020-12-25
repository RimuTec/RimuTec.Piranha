using System;
using System.Collections.Generic;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal sealed class PageEntity : RoutedContentBaseEntity<PageFieldEntity>
   {
      /// <summary>
      /// Gets/sets the page type id.
      /// </summary>
      public string PageTypeId { get; set; }

      /// <summary>
      /// Gets/sets the site id.
      /// </summary>
      public Guid SiteId { get; set; }

      /// <summary>
      /// Gets/sets the optional parent id. Used to
      /// position the page in the sitemap.
      /// </summary>
      public Guid? ParentId { get; set; }

      /// <summary>
      /// Gets/sets the type of content this page
      /// contains.
      /// </summary>
      public string ContentType { get; set; }

      /// <summary>
      /// Gets/sets the optional primary image id.
      /// </summary>
      public Guid? PrimaryImageId { get; set; }

      /// <summary>
      /// Gets/sets the optional excerpt.
      /// </summary>
      public string Excerpt { get; set; }

      /// <summary>
      /// Gets/sets the pages sort order in its
      /// hierarchical position.
      /// </summary>
      public int SortOrder { get; set; }

      /// <summary>
      /// Gets/sets the optional navigation title.
      /// </summary>
      public string NavigationTitle { get; set; }

      /// <summary>
      /// Gets/sets if the page should be visible
      /// in the navigation.
      /// </summary>
      public bool IsHidden { get; set; }

      /// <summary>
      /// Gets/sets the optional redirect.
      /// </summary>
      /// <returns></returns>
      public string RedirectUrl { get; set; }

      /// <summary>
      /// Gets/sets the redirect type.
      /// </summary>
      /// <returns></returns>
      public Piranha.Models.RedirectType RedirectType { get; set; } = Piranha.Models.RedirectType.Temporary;

      /// <summary>
      /// Gets/sets if comments should be enabled.
      /// </summary>
      /// <value></value>
      public bool EnableComments { get; set; }

      /// <summary>
      /// Gets/sets after how many days after publish date comments
      /// should be closed. A value of 0 means never.
      /// </summary>
      public int CloseCommentsAfterDays { get; set; }

      /// <summary>
      /// Gets/sets the site.
      /// </summary>
      public SiteEntity Site { get; set; }

      /// <summary>
      /// Gets/sets the associated page type.
      /// </summary>
      public PageTypeEntity PageType { get; set; }

      /// <summary>
      /// Gets/sets the optional page.
      /// </summary>
      public PageEntity Parent { get; set; }

      /// <summary>
      /// Gets/sets the available page blocks.
      /// </summary>
      public IList<PageBlockEntity> Blocks { get; set; } = new List<PageBlockEntity>();

      /// <summary>
      /// Gets/sets the available permissions.
      /// </summary>
      public IList<PagePermissionEntity> Permissions { get; set; } = new List<PagePermissionEntity>();

      /// <summary>
      /// Gets/sets the optional page this page is a copy of
      /// </summary>
      public Guid? OriginalPageId { get; set; }
   }
}
