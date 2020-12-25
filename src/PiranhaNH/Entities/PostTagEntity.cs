using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal sealed class PostTagEntity
   {
      /// <summary>
      /// Gets/sets the post id.
      /// </summary>
      public Guid PostId { get; set; }

      /// <summary>
      /// Gets/sets the tag id.
      /// </summary>
      public Guid TagId { get; set; }

      /// <summary>
      /// Gets/sets the post.
      /// </summary>
      [JsonIgnore]
      public PostEntity Post { get; set; }

      /// <summary>
      /// Gets/sets the tag.
      /// </summary>
      public TagEntity Tag { get; set; }
   }
}
