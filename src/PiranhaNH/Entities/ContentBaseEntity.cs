using System.Collections.Generic;

namespace RimuTec.PiranhaNH.Entities
{
    internal abstract class ContentBaseEntity<T> : EntityBase where T : ContentFieldBase
    {
        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
	    public virtual string Title { get; set; }

        /// <summary>
        /// Gets/sets the available fields.
        /// </summary>
        public virtual IList<T> Fields { get; set; } = new List<T>();
    }
}
