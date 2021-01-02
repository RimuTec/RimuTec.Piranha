using System;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal abstract class RoutedContentBaseEntity<T> : ContentBaseEntity<T> where T : ContentFieldBaseEntity
   {
      /// <summary>
      /// Gets/sets the unique slug.
      /// </summary>
      public virtual string Slug { get; set; }

      /// <summary>
      /// Gets/sets the optional meta title.
      /// </summary>
      public virtual string MetaTitle { get; set; }

      /// <summary>
      /// Gets/sets the optional meta keywords.
      /// </summary>
      public virtual string MetaKeywords { get; set; }

      /// <summary>
      /// Gets/sets the optional meta description.
      /// </summary>
      public virtual string MetaDescription { get; set; }

      /// <summary>
      /// Gets/sets the optional open graph title.
      /// </summary>
      public virtual string OgTitle { get; set; }

      /// <summary>
      /// Gets/sets the optional open graph description.
      /// </summary>
      public virtual string OgDescription { get; set; }

      /// <summary>
      /// Gets/sets the optional open graph image.
      /// </summary>
      public virtual Guid OgImageId { get; set; }

      /// <summary>
      /// Gets/sets the optional route.
      /// </summary>
      public virtual string Route { get; set; }

      /// <summary>
      /// Gets/sets the publishe date.
      /// </summary>
      public virtual DateTime? Published { get; set; }
   }
}
