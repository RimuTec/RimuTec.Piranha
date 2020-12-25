using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal sealed class CategoryEntity : TaxonomyEntity
   {
      /// <summary>
      /// Gets/sets the id of the blog page this
      /// category belongs to.
      /// </summary>
      public Guid BlogId { get; set; }

      /// <summary>
      /// Gets/sets the blog page this category belongs to.
      /// </summary>
      [JsonIgnore]
      public PageEntity Blog { get; set; }
   }
}
