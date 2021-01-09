using System;

namespace RimuTec.PiranhaNH.Entities
{
   internal abstract class CommentEntity : EntityBase
   {
      //   /// <summary>
      //   /// Gets/sets the unique id.
      //   /// </summary>
      //   public virtual Guid Id { get; private set; }

      //   /// <summary>
      //   /// Gets/sets the created date.
      //   /// </summary>
      //   public virtual DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the optional user id.
        /// </summary>
        public virtual string UserId { get; set; }

        /// <summary>
        /// Gets/sets the author name.
        /// </summary>
        public virtual string Author { get; set; }

        /// <summary>
        /// Gets/sets the email address.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets/sets the optional website URL.
        /// </summary>
        public virtual string Url { get; set; }

        /// <summary>
        /// Gets/sets if the comment has been approved. Comments are
        /// approved by default unless you use some kind of comment
        /// validation mechanism.
        /// </summary>
        public virtual bool IsApproved { get; set; } = true;

        /// <summary>
        /// Gets/sets the comment body.
        /// </summary>
        public virtual string Body { get; set; }
   }
}
