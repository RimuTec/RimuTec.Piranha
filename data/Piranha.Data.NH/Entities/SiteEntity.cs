using System;

namespace RimuTec.Piranha.Data.NH.Entities
{
    internal class SiteEntity : ContentBaseEntity<SiteFieldEntity>
    {
        /// <summary>
        /// Gets/sets the optional site type id.
        /// </summary>
        public virtual string SiteTypeId { get; set; }

        /// <summary>
        /// Gets/sets the internal textual id.
        /// </summary>
        public virtual string InternalId { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets/sets the optional logo image id.
        /// </summary>
        public virtual Guid? LogoId { get; set; }

        /// <summary>
        /// Gets/sets the optional hostnames to bind this site for.
        /// </summary>
        public virtual string Hostnames { get; set; }

        /// <summary>
        /// Gets/sets if this is the default site.
        /// </summary>
        public virtual bool IsDefault { get; set; }

        /// <summary>
        /// Gets/sets the optional culture for the site.
        /// </summary>
        public virtual string Culture { get; set; }

        /// <summary>
        /// Gets/sets the global last modification date
        /// of the site's content.
        /// </summary>
        public virtual DateTime? ContentLastModified { get; set; }
    }
}
