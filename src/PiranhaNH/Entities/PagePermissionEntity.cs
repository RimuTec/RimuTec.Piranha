using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   [Serializable]
   internal sealed class PagePermissionEntity
   {
      public Guid PageId { get; set; }
      public string Permission { get; set; }

      [JsonIgnore]
      public PageEntity Page { get; set; }
   }
}
