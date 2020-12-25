using System;
using Newtonsoft.Json;

namespace RimuTec.PiranhaNH.Entities
{
   /// <summary>
   /// Content field for a block.
   /// </summary>
   [Serializable]
   public sealed class BlockFieldEntity
   {
      /// <summary>
      /// Gets/sets the unique id.
      /// </summary>
      public Guid Id { get; set; }

      /// <summary>
      /// Gets/sets the id of the block this field
      /// belongs to.
      /// </summary>
      public Guid BlockId { get; set; }

      /// <summary>
      /// Gets/sets the field id.
      /// </summary>
      public string FieldId { get; set; }

      /// <summary>
      /// Gets/sets the sort index if the block
      /// is a collection.
      /// </summary>
      public int SortOrder { get; set; }

      /// <summary>
      /// Gets/sets the CLR type of the field.
      /// </summary>
      public string CLRType { get; set; }

      /// <summary>
      /// Gets/sets the serialized field value.
      /// </summary>
      /// <returns></returns>
      public string Value { get; set; }

      /// <summary>
      /// Gets/sets the block containing the field.
      /// </summary>
      [JsonIgnore]
      public BlockEntity Block { get; set; }
   }
}
