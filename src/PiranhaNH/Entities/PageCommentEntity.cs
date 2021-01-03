namespace RimuTec.PiranhaNH.Entities
{
   internal class PageCommentEntity : CommentEntity
   {
      //   /// <summary>
      //   /// Gets/sets the page id.
      //   /// </summary>
      //   public virtual Guid PageId { get; set; }

        /// <summary>
        /// Gets/sets the page.
        /// </summary>
        public virtual PageEntity Page { get; set; }
   }
}
