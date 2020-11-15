using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
    internal class AliasEntityMap : ClassMap<AliasEntity>
    {
        public AliasEntityMap()
        {
            Table("Alias");

            Id(e => e.Id);
            Map(e => e.Created);
            Map(e => e.LastModified);

            Map(e => e.AliasUrl);
            Map(e => e.RedirectUrl);
            Map(e => e.Type);

            References(e => e.Site);
        }
    }
}
