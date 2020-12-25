using System;
using System.Collections.Generic;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal sealed class PostEntity : RoutedContentBaseEntity<PostFieldEntity>
   {
      /// <summary>
      /// Gets/sets the post type id.
      /// </summary>
      public string PostTypeId { get; set; }

      /// <summary>
      /// Gets/sets the id of the blog page this
      /// post belongs to.
      /// </summary>
      public Guid BlogId { get; set; }

      /// <summary>
      /// Gets/sets the category id.
      /// </summary>
      public Guid CategoryId { get; set; }

      /// <summary>
      /// Gets/sets the optional primary image id.
      /// </summary>
      public Guid? PrimaryImageId { get; set; }

      /// <summary>
      /// Gets/sets the optional excerpt.
      /// </summary>
      public string Excerpt { get; set; }

      /// <summary>
      /// Gets/sets the optional redirect.
      /// </summary>
      /// <returns></returns>
      public string RedirectUrl { get; set; }

      /// <summary>
      /// Gets/sets the redirect type.
      /// </summary>
      /// <returns></returns>
      public Piranha.Models.RedirectType RedirectType { get; set; }

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
      /// Gets/sets the associated post type.
      /// </summary>
      public PostTypeEntity PostType { get; set; }

      /// <summary>
      /// Gets/sets the blog page this category belongs to.
      /// </summary>
      public PageEntity Blog { get; set; }

      /// <summary>
      /// Gets/sets the post category.
      /// </summary>
      public CategoryEntity Category { get; set; }

      /// <summary>
      /// Gets/sets the available tags.
      /// </summary>
      public IList<PostTagEntity> Tags { get; set; } = new List<PostTagEntity>();

      /// <summary>
      /// Gets/sets the available post blocks.
      /// </summary>
      public IList<PostBlockEntity> Blocks { get; set; } = new List<PostBlockEntity>();

      /// <summary>
      /// Gets/sets the available permissions.
      /// </summary>
      public IList<PostPermissionEntity> Permissions { get; set; } = new List<PostPermissionEntity>();
   }
}
