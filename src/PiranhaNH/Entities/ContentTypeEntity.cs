using System;

namespace RimuTec.PiranhaNH.Entities
{
   internal abstract class ContentTypeEntity
   {
      public virtual string Id { get; set; }
      public virtual DateTime Created { get; protected internal set; }
      public virtual DateTime LastModified { get; protected internal set; }
      /// <summary>
      /// Gets/sets the CLR type of the content model.
      /// </summary>
      public virtual string CLRType { get; set; }

      /// <summary>
      /// Gets/sets the JSON serialized body of the post type.
      /// </summary>
      public virtual string Body { get; set; }
   }
}
