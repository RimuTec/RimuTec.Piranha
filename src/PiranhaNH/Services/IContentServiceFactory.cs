using AutoMapper;
using RimuTec.PiranhaNH.Entities;

namespace RimuTec.PiranhaNH.Services
{
   internal interface IContentServiceFactory
   {
      /// <summary>
      /// Creates a new content service for the specified types.
      /// </summary>
      /// <param name="mapper">The AutoMapper instance to use for transformation</param>
      /// <returns>The content service</returns>
      IContentService<TContent, TField, TModelBase> Create<TContent, TField, TModelBase>(IMapper mapper)
          where TContent : ContentBaseEntity<TField>
          where TField : ContentFieldBaseEntity
          where TModelBase : Piranha.Models.ContentBase;

      /// <summary>
      /// Creates a new page content service.
      /// </summary>
      /// <returns>The content service</returns>
      IContentService<PageEntity, PageFieldEntity, Piranha.Models.PageBase> CreatePageService();

      /// <summary>
      /// Creates a new post content service.
      /// </summary>
      /// <returns>The content service</returns>
      IContentService<PostEntity, PostFieldEntity, Piranha.Models.PostBase> CreatePostService();

      /// <summary>
      /// Creates a new site content service.
      /// </summary>
      /// <returns>The content service</returns>
      IContentService<SiteEntity, SiteFieldEntity, Piranha.Models.SiteContentBase> CreateSiteService();
   }
}
