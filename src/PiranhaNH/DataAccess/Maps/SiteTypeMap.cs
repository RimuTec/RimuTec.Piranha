using FluentNHibernate.Mapping;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.DataAccess.Maps
{
    internal class SiteTypeMap : ClassMap<SiteTypeEntity>
    {
        public SiteTypeMap()
        {
            Table("SiteType");

            Id(e => e.Id);
            Map(e => e.Created);
            Map(e => e.LastModified);

            Map(e => e.Body);
            Map(e => e.CLRType);
        }
    }
}
