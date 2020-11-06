using System;

namespace RimuTec.Piranha.Data.NH.Entities
{
    internal abstract class ContentFieldBase
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the region id.
        /// </summary>
        public virtual string RegionId { get; set; }

        /// <summary>
        /// Gets/sets the field id.
        /// </summary>
        public virtual string FieldId { get; set; }

        /// <summary>
        /// Gets/sets the sort order.
        /// </summary>
        public virtual int SortOrder { get; set; }

        /// <summary>
        /// Gets/sets the sort order of the value.
        /// </summary>
        public virtual string CLRType { get; set; }

        /// <summary>
        /// Gets/sets the JSON serialized value.
        /// </summary>
        public virtual string Value { get; set; }
    }
}
