using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Piranha;
using Piranha.Extend;
using Piranha.Models;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH
{
   public class Module : IModule
   {
      public string Author => "RimuTec Ltd";

      public string Name => GetType().Assembly.FullName.Split(',')[0];

      public string Version => Utils.GetAssemblyVersion(GetType().Assembly);

      public string Description => "Data implementation for NHibnernate.";

      public string PackageUrl => "tbd";

      public string IconUrl => "tbd";

      public void Init()
      {
      }

      static Module()
      {
         var mapperConfig = new MapperConfiguration(cfg =>
         {
            cfg
               .CreateMap<PageEntity, PageEntity>()
               .ForMember(d => d.PageType, o => o.MapFrom(s => s.PageType))
               .ForMember(d => d.Blocks, o => o.MapFrom(s => new List<PageBlockEntity>(s.Blocks)))
               .ForAllOtherMembers(o => o.Ignore())
               ;
            cfg
               .CreateMap<AliasEntity, AliasEntity>()
               .ForMember(a => a.Id, o => o.Ignore())
               .ForMember(a => a.Created, o => o.Ignore())
               ;
            cfg.CreateMap<CategoryEntity, CategoryEntity>()
                   .ForMember(c => c.Id, o => o.Ignore())
                   .ForMember(c => c.Created, o => o.Ignore())
                   ;
            cfg.CreateMap<CategoryEntity, Taxonomy>()
                   .ForMember(c => c.Type, o => o.MapFrom(_ => TaxonomyType.Category))
                  //  cfg.CreateMap<Data.MediaFolder, Data.MediaFolder>()
                  //      .ForMember(f => f.Id, o => o.Ignore())
                  //      .ForMember(f => f.Created, o => o.Ignore())
                  //      .ForMember(f => f.Media, o => o.Ignore());
                  //  cfg.CreateMap<Data.MediaFolder, Models.MediaStructureItem>()
                  //      .ForMember(f => f.Level, o => o.Ignore())
                  //      .ForMember(f => f.FolderCount, o => o.Ignore())
                  //      .ForMember(f => f.MediaCount, o => o.Ignore())
                  //      .ForMember(f => f.Items, o => o.Ignore());
                  ;
            cfg.CreateMap<PageEntity, PageBase>()
                .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PageType.Id))
                .ForMember(p => p.PrimaryImage, o => o.MapFrom(m => m.PrimaryImageId))
                .ForMember(p => p.OgImage, o => o.MapFrom(m => m.OgImageId))
                .ForMember(p => p.Permalink, o => o.MapFrom(m => "/" + m.Slug))
                .ForMember(p => p.Permissions, o => o.Ignore())
                .ForMember(p => p.Blocks, o => o.Ignore())
                .ForMember(p => p.CommentCount, o => o.Ignore())
                ;
            cfg.CreateMap<PageBase, PageEntity>()
                   .ForMember(p => p.ContentType, o => o.Ignore())
                   .ForMember(p => p.PrimaryImageId, o => o.MapFrom(m => m.PrimaryImage != null ? m.PrimaryImage.Id : null))
                   .ForMember(p => p.OgImageId, o => o.MapFrom(m => m.OgImage != null ? m.OgImage.Id : null))
                   .ForMember(p => p.PageType, o => o.Ignore())
                   //.ForMember(p => p.PageTypeId, o => o.MapFrom(m => m.TypeId))
                   .ForMember(p => p.Blocks, o => o.Ignore())
                   .ForMember(p => p.Fields, o => o.Ignore())
                   .ForMember(p => p.Comments, o => o.Ignore())
                   .ForMember(p => p.Created, o => o.Ignore())
                   .ForMember(p => p.LastModified, o => o.Ignore())
                   .ForMember(p => p.Permissions, o => o.Ignore())
                   .ForMember(p => p.PageType, o => o.Ignore())
                   .ForMember(p => p.Site, o => o.Ignore())
                   .ForMember(p => p.Parent, o => o.Ignore())
                   .ForMember(p => p.RevisionNumber, o => o.Ignore())
                   ;
            cfg.CreateMap<PageEntity, SitemapItem>()
                   .ForMember(p => p.MenuTitle, o => o.Ignore())
                   .ForMember(p => p.Level, o => o.Ignore())
                   .ForMember(p => p.Items, o => o.Ignore())
                   .ForMember(p => p.PageTypeName, o => o.Ignore())
                   .ForMember(p => p.Permalink, o => o.MapFrom(d => d.Parent == null && d.SortOrder == 0 ? "/" : "/" + d.Slug))
                   //.ForMember(p => p.Permalink, o => o.MapFrom(d => !d.ParentId.HasValue && d.SortOrder == 0 ? "/" : "/" + d.Slug))
                   .ForMember(p => p.Permissions, o => o.MapFrom(d => d.Permissions.Select(dp => dp.Permission).ToList()))
            //  cfg.CreateMap<ParamEntity, ParamEntity>()
            //      .ForMember(p => p.Id, o => o.Ignore())
            //      .ForMember(p => p.Created, o => o.Ignore());
                  ;
            cfg.CreateMap<PostEntity, PostBase>()
              .ForMember(p => p.TypeId, o => o.MapFrom(m => m.PostTypeId))
              .ForMember(p => p.PrimaryImage, o => o.MapFrom(m => m.PrimaryImageId))
              .ForMember(p => p.OgImage, o => o.MapFrom(m => m.OgImageId))
              .ForMember(p => p.Permalink, o => o.Ignore())
              .ForMember(p => p.Permissions, o => o.Ignore())
              .ForMember(p => p.Blocks, o => o.Ignore())
              .ForMember(p => p.CommentCount, o => o.Ignore())
              ;
            cfg.CreateMap<PostTagEntity, Taxonomy>()
                   .ForMember(p => p.Id, o => o.MapFrom(m => m.TagId))
                   .ForMember(p => p.Title, o => o.MapFrom(m => m.Tag.Title))
                   .ForMember(p => p.Slug, o => o.MapFrom(m => m.Tag.Slug))
                   .ForMember(p => p.Type, o => o.MapFrom(_ => TaxonomyType.Tag))
                   ;
            cfg.CreateMap<PostBase, PostEntity>()
                   .ForMember(p => p.PostTypeId, o => o.MapFrom(m => m.TypeId))
                   .ForMember(p => p.CategoryId, o => o.MapFrom(m => m.Category.Id))
                   .ForMember(p => p.PrimaryImageId, o => o.MapFrom(m => m.PrimaryImage != null ? m.PrimaryImage.Id : null))
                   .ForMember(p => p.OgImageId, o => o.MapFrom(m => m.OgImage != null ? m.OgImage.Id : null))
                   .ForMember(p => p.Blocks, o => o.Ignore())
                   .ForMember(p => p.Fields, o => o.Ignore())
                   .ForMember(p => p.Created, o => o.Ignore())
                   .ForMember(p => p.LastModified, o => o.Ignore())
                   .ForMember(p => p.Permissions, o => o.Ignore())
                   .ForMember(p => p.PostType, o => o.Ignore())
                   .ForMember(p => p.Blog, o => o.Ignore())
                   .ForMember(p => p.Category, o => o.Ignore())
                   .ForMember(p => p.Tags, o => o.Ignore())
                   ;
            cfg.CreateMap<SiteEntity, SiteEntity>()
                   .ForMember(s => s.Id, o => o.Ignore())
                   .ForMember(s => s.Created, o => o.Ignore())
                   ;
            cfg.CreateMap<SiteEntity, SiteContentBase>()
                   .ForMember(s => s.TypeId, o => o.MapFrom(m => m.SiteTypeId))
                   .ForMember(s => s.Permissions, o => o.Ignore())
                   ;
            cfg.CreateMap<SiteContentBase, SiteEntity>()
                   .ForMember(s => s.Id, o => o.Ignore())
                   .ForMember(s => s.SiteTypeId, o => o.Ignore())
                   .ForMember(s => s.InternalId, o => o.Ignore())
                   .ForMember(s => s.Description, o => o.Ignore())
                   .ForMember(s => s.LogoId, o => o.Ignore())
                   .ForMember(s => s.Hostnames, o => o.Ignore())
                   .ForMember(s => s.IsDefault, o => o.Ignore())
                   .ForMember(s => s.Culture, o => o.Ignore())
                   .ForMember(s => s.Fields, o => o.Ignore())
                   .ForMember(s => s.Created, o => o.Ignore())
                   .ForMember(s => s.LastModified, o => o.Ignore())
                   .ForMember(s => s.ContentLastModified, o => o.Ignore())
                   ;
            cfg.CreateMap<TagEntity, TagEntity>()
                   .ForMember(t => t.Id, o => o.Ignore())
                   .ForMember(t => t.Created, o => o.Ignore())
                   ;
            cfg.CreateMap<TagEntity, Taxonomy>()
                   .ForMember(t => t.Type, o => o.MapFrom(_ => TaxonomyType.Tag))
                   ;
         });
         mapperConfig.AssertConfigurationIsValid();
         Mapper = mapperConfig.CreateMapper();
      }

      public static IMapper Mapper { get; }
   }
}
