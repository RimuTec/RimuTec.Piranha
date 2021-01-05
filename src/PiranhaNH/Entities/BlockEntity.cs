using System;
using System.Collections.Generic;

namespace RimuTec.PiranhaNH.Entities
{
   /// <summary>
   /// Reusable content block.
   /// </summary>
   [Serializable]
   internal class BlockEntity : EntityBase
   {
      // /// <summary>
      // /// Gets/sets the unique id.
      // /// </summary>
      // public virtual Guid Id { get; private set; }

      // /// <summary>
      // /// Gets/sets the created date.
      // /// </summary>
      // public virtual DateTime Created { get; set; }

      // /// <summary>
      // /// Gets/sets the last modification date.
      // /// </summary>
      // public virtual DateTime LastModified { get; set; }

      /// <summary>
      /// This is not part of the data model. It's only used
      /// for internal mapping.
      /// </summary>
      public virtual Guid? ParentId { get; set; }

      /// <summary>
      /// Gets/sets the CLR type of the block.
      /// </summary>
      public virtual string CLRType { get; set; }

      /// <summary>
      /// Gets/sets the optional title. This property
      /// is only used for reusable blocks within the
      /// block library.
      /// </summary>
      public virtual string Title { get; set; }

      /// <summary>
      /// Gets/sets if this is a reusable block.
      /// </summary>
      public virtual bool IsReusable { get; set; }

      /// <summary>
      /// Gets/sets the available fields.
      /// </summary>
      public virtual IList<BlockFieldEntity> Fields { get; set; } = new List<BlockFieldEntity>();
   }
}
