using System;

namespace RimuTec.PiranhaNH.Entities
{
    /// <summary>
    /// Connection between a page and a content.
    /// </summary>
    internal interface IContentBlockEntity
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the block id.
        /// </summary>
        Guid BlockId { get; set; }

        /// <summary>
        /// Gets/sets the zero based sort index.
        /// </summary>
        int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the block data.
        /// </summary>
        BlockEntity Block { get; set; }
    }
}
