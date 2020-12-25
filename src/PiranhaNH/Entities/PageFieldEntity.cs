using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal sealed class PageFieldEntity : ContentFieldBaseEntity
   {
      /// <summary>
      /// Gets/sets the page id.
      /// </summary>
      public Guid PageId { get; set; }

      /// <summary>
      /// Gets/sets the page.
      /// </summary>
      [JsonIgnore]
      public PageEntity Page { get; set; }
   }
}
