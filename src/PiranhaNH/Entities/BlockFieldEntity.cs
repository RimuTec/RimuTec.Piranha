using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   /// <summary>
   /// Content field for a block.
   /// </summary>
   [Serializable]
   internal class BlockFieldEntity : EntityBase
   {
      // /// <summary>
      // /// Gets/sets the unique id.
      // /// </summary>
      // public virtual Guid Id { get; private set; }

      // /// <summary>
      // /// Gets/sets the id of the block this field
      // /// belongs to.
      // /// </summary>
      // public virtual Guid BlockId { get; set; }

      /// <summary>
      /// Gets/sets the field id.
      /// </summary>
      public virtual string FieldId { get; set; }

      /// <summary>
      /// Gets/sets the sort index if the block
      /// is a collection.
      /// </summary>
      public virtual int SortOrder { get; set; }

      /// <summary>
      /// Gets/sets the CLR type of the field.
      /// </summary>
      public virtual string CLRType { get; set; }

      /// <summary>
      /// Gets/sets the serialized field value.
      /// </summary>
      /// <returns></returns>
      public virtual string Value { get; set; }

      /// <summary>
      /// Gets/sets the block containing the field.
      /// </summary>
      [JsonIgnore]
      public virtual BlockEntity Block { get; set; }
   }
}
