using System;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal abstract class TaxonomyEntity : EntityBase
   {
      /// <summary>
      /// Gets/sets the title.
      /// </summary>
      public string Title { get; set; }

      /// <summary>
      /// Gets/sets the slug.
      /// </summary>
      public string Slug { get; set; }
   }
}
