using System;

namespace RimuTec.PiranhaNH.Entities
{
    internal class EntityBase
    {
        public virtual Guid Id { get; private set; }
        public virtual DateTime Created { get; protected internal set; }
        public virtual DateTime LastModified { get; protected internal set; }
    }
}
