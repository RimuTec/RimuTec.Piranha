using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   /// <summary>
   /// Connection between a post and a block.
   /// </summary>
   [Serializable]
   internal sealed class PostBlockEntity : IContentBlockEntity
   {
      /// <summary>
      /// Gets/sets the unique id.
      /// </summary>
      public Guid Id { get; private set; }

      /// <summary>
      /// This property is not used any more, but is kept for atm
      /// backwards compatible SQLite migrations.
      /// </summary>
      public Guid? ParentId { get; set; }

      /// <summary>
      /// Gets/sets the post id.
      /// </summary>
      public Guid PostId { get; set; }

      /// <summary>
      /// Gets/sets the block id.
      /// </summary>
      public Guid BlockId { get; set; }

      /// <summary>
      /// Gets/sets the zero based sort index.
      /// </summary>
      public int SortOrder { get; set; }

      /// <summary>
      /// Gets/sets the post containing the block.
      /// </summary>
      [JsonIgnore]
      public PostEntity Post { get; set; }

      /// <summary>
      /// Gets/sets the block data.
      /// </summary>
      public BlockEntity Block { get; set; }
   }
}
