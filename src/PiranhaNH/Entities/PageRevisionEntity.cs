namespace RimuTec.PiranhaNH.Entities
{
   internal class PageRevisionEntity : ContentRevisionBaseEntity
   {
      //   /// <summary>
      //   /// Gets/sets the id of the page this revision
      //   /// belongs to.
      //   /// </summary>
      //   public Guid PageId { get; set; }

        /// <summary>
        /// Gets/sets the page this revision belongs to.
        /// </summary>
        public PageEntity Page { get; set; }
   }
}
