using System;

namespace RimuTec.PiranhaNH.Entities
{
   internal class EntityBase : IHaveAuditInformation
   {
      public virtual Guid Id { get; private set; }
      public virtual DateTime Created { get; set; }
      public virtual DateTime LastModified { get; set; }
   }
}
