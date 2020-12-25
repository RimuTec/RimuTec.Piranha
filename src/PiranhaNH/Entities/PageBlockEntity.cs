using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   /// <summary>
   /// Connection between a page and a block.
   /// </summary>
   [Serializable]
   internal sealed class PageBlockEntity : IContentBlockEntity
   {
      /// <summary>
      /// Gets/sets the unique id.
      /// </summary>
      public Guid Id { get; set; }

      /// <summary>
      /// This property is not used any more, but is kept for atm
      /// backwards compatible SQLite migrations.
      /// </summary>
      public Guid? ParentId { get; set; }

      /// <summary>
      /// Gets/sets the page id.
      /// </summary>
      public Guid PageId { get; set; }

      /// <summary>
      /// Gets/sets the block id.
      /// </summary>
      public Guid BlockId { get; set; }

      /// <summary>
      /// Gets/sets the zero based sort index.
      /// </summary>
      public int SortOrder { get; set; }

      /// <summary>
      /// Gets/sets the page containing the block.
      /// </summary>
      [JsonIgnore]
      public PageEntity Page { get; set; }

      /// <summary>
      /// Gets/sets the block data.
      /// </summary>
      public BlockEntity Block { get; set; }
   }
}
