using System.ComponentModel.DataAnnotations;

namespace RimuTec.PiranhaNH.Entities
{
   internal class ParamEntity : EntityBase
   {
        /// <summary>
        /// Gets/sets the unique key.
        /// </summary>
        [Required]
        [StringLength(64)]
        public virtual string Key { get; set; }

        /// <summary>
        /// Gets/sets the value.
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets/sets the optional description.
        /// </summary>
        [StringLength(255)]
        public virtual string Description { get; set; }
   }
}
