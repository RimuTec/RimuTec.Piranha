using AutoMapper;
using Piranha.Models;
using Piranha.Services;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.Services
{
   internal class ContentServiceFactory : IContentServiceFactory
   {
      public ContentServiceFactory(IContentFactory factory)
      {
         _factory = factory;
      }
      public IContentService<TContent, TField, TModelBase> Create<TContent, TField, TModelBase>(IMapper mapper)
         where TContent : ContentBaseEntity<TField>
         where TField : ContentFieldBaseEntity
         where TModelBase : ContentBase
      {
         throw new System.NotImplementedException();
      }

      public IContentService<PageEntity, PageFieldEntity, PageBase> CreatePageService()
      {
         return new ContentService<PageEntity, PageFieldEntity, PageBase>(_factory, Module.Mapper);
      }

      public IContentService<PostEntity, PostFieldEntity, PostBase> CreatePostService()
      {
         throw new System.NotImplementedException();
      }

      public IContentService<SiteEntity, SiteFieldEntity, SiteContentBase> CreateSiteService()
      {
          return new ContentService<SiteEntity, SiteFieldEntity, SiteContentBase>(_factory, Module.Mapper);
      }

      private readonly IContentFactory _factory;
   }
}
