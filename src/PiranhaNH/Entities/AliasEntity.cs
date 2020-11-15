using Piranha.Models;

namespace RimuTec.PiranhaNH.Entities
{
    internal class AliasEntity : EntityBase
    {
        /// <summary>
        /// Gets/sets the site this alias is for.
        /// </summary>
        public virtual SiteEntity Site { get; set; }

        /// <summary>
        /// Gets/sets the alias url.
        /// </summary>
        public virtual string AliasUrl { get; set; }

        /// <summary>
        /// Gets/sets the url of the redirect.
        /// </summary>
        public virtual string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets if this is a permanent or temporary
        /// redirect.
        /// </summary>
        public virtual RedirectType Type { get; set; } = RedirectType.Temporary;
    }
}
