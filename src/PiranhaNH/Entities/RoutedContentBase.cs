using System;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal abstract class RoutedContentBaseEntity<T> : ContentBaseEntity<T> where T : ContentFieldBaseEntity
   {
      /// <summary>
      /// Gets/sets the unique slug.
      /// </summary>
      public string Slug { get; set; }

      /// <summary>
      /// Gets/sets the optional meta title.
      /// </summary>
      public string MetaTitle { get; set; }

      /// <summary>
      /// Gets/sets the optional meta keywords.
      /// </summary>
      public string MetaKeywords { get; set; }

      /// <summary>
      /// Gets/sets the optional meta description.
      /// </summary>
      public string MetaDescription { get; set; }

      /// <summary>
      /// Gets/sets the optional open graph title.
      /// </summary>
      public string OgTitle { get; set; }

      /// <summary>
      /// Gets/sets the optional open graph description.
      /// </summary>
      public string OgDescription { get; set; }

      /// <summary>
      /// Gets/sets the optional open graph image.
      /// </summary>
      public Guid OgImageId { get; set; }

      /// <summary>
      /// Gets/sets the optional route.
      /// </summary>
      public string Route { get; set; }

      /// <summary>
      /// Gets/sets the publishe date.
      /// </summary>
      public DateTime? Published { get; set; }
   }
}
