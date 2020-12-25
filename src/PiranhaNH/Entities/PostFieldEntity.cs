using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal sealed class PostFieldEntity : ContentFieldBaseEntity
   {
      /// <summary>
      /// Gets/sets the post id.
      /// </summary>
      public Guid PostId { get; set; }

      /// <summary>
      /// Gets/sets the post.
      /// </summary>
      [JsonIgnore]
      public PostEntity Post { get; set; }
   }
}
