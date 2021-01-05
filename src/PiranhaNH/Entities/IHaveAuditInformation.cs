using System;

namespace RimuTec.PiranhaNH.Entities
{
   internal interface IHaveAuditInformation
   {
      DateTime Created { get; set; }
      DateTime LastModified { get; set; }
   }
}
