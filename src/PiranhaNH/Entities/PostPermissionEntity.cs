using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal sealed class PostPermissionEntity
   {
      public Guid PostId { get; set; }
      public string Permission { get; set; }

      [JsonIgnore]
      public PostEntity Post { get; set; }
   }
}
