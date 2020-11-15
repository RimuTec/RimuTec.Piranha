using FluentNHibernate.Mapping;
using RimuTec.Piranha.Data.NH.Entities;

namespace RimuTec.Piranha.Data.NH.DataAccess.Maps
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
