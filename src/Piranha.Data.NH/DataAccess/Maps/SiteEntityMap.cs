using FluentNHibernate.Mapping;
using RimuTec.Piranha.Data.NH.Entities;

namespace RimuTec.Piranha.Data.NH.DataAccess.Maps
{
    internal class SiteEntityMap : ClassMap<SiteEntity>
    {
        public SiteEntityMap()
        {
            Table("Site");

            Id(e => e.Id);
            Map(e => e.Created);
            Map(e => e.LastModified);

            Map(e => e.ContentLastModified);
            Map(e => e.Culture);
            Map(e => e.Description);
            Map(e => e.Hostnames);
            Map(e => e.InternalId);
            Map(e => e.IsDefault);
            Map(e => e.LogoId);
            Map(e => e.SiteTypeId);
            Map(e => e.Title);

            HasMany(e => e.Fields);
        }
    }
}
