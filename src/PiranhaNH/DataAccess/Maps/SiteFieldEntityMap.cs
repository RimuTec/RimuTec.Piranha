using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
    internal class SiteFieldEntityMap : ClassMap<SiteFieldEntity>
    {
        public SiteFieldEntityMap()
        {
            Table("SiteField");

            Id(e => e.Id);

            Map(e => e.RegionId);
            Map(e => e.FieldId);
            Map(e => e.SortOrder);
            Map(e => e.CLRType);
            Map(e => e.Value);

            References(e => e.Site);
        }
    }
}
