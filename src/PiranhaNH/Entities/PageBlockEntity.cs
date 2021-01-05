using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   /// <summary>
   /// Connection between a page and a block.
   /// </summary>
   [Serializable]
   internal class PageBlockEntity : EntityBase, IContentBlockEntity
   {
      // /// <summary>
      // /// Gets/sets the unique id.
      // /// </summary>
      // public virtual Guid Id { get; private set; }

      /// <summary>
      /// This property is not used any more, but is kept for atm
      /// backwards compatible SQLite migrations.
      /// </summary>
      public virtual Guid? ParentId { get; set; }

      // /// <summary>
      // /// Gets/sets the page id.
      // /// </summary>
      // public virtual Guid PageId { get; set; }

      // /// <summary>
      // /// Gets/sets the block id.
      // /// </summary>
      // public virtual Guid BlockId { get; set; }

      /// <summary>
      /// Gets/sets the zero based sort index.
      /// </summary>
      public virtual int SortOrder { get; set; }

      /// <summary>
      /// Gets/sets the page containing the block.
      /// </summary>
      [JsonIgnore]
      public virtual PageEntity Page { get; set; }

      /// <summary>
      /// Gets/sets the block data.
      /// </summary>
      public virtual BlockEntity Block { get; set; }
   }
}
