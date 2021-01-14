using System;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.Repositories;
using Piranha.Services;
using RimuTec.PiranhaNH.Repositories;
using RimuTec.PiranhaNH.Services;

namespace RimuTec.PiranhaNH
{
   public static class PiranhaNHExtensions
   {
      public static IServiceCollection AddPiranhaNH(this IServiceCollection services, ServiceLifetime scope = ServiceLifetime.Scoped)
      {
         // Add this module:
         try 
         {
            App.Modules.Register<Module>();
         }
         catch(Exception ex)
         {
            throw;
         }

         // Register repositories:
         services.Add(new ServiceDescriptor(typeof(IAliasRepository), typeof(AliasRepository), scope));
         //   services.Add(new ServiceDescriptor(typeof(IArchiveRepository), typeof(ArchiveRepository), scope));
         //   services.Add(new ServiceDescriptor(typeof(IMediaRepository), typeof(MediaRepository), scope));
         services.Add(new ServiceDescriptor(typeof(IPageRepository), typeof(PageRepository), scope));
         services.Add(new ServiceDescriptor(typeof(IPageTypeRepository), typeof(PageTypeRepository), scope));
         services.Add(new ServiceDescriptor(typeof(IParamRepository), typeof(ParamRepository), scope));
         //services.Add(new ServiceDescriptor(typeof(IPostRepository), typeof(PostRepository), scope));
         services.Add(new ServiceDescriptor(typeof(IPostTypeRepository), typeof(PostTypeRepository), scope));
         services.Add(new ServiceDescriptor(typeof(ISiteRepository), typeof(SiteRepository), scope));
         services.Add(new ServiceDescriptor(typeof(ISiteTypeRepository), typeof(SiteTypeRepository), scope));

         // Register services:
         services.Add(new ServiceDescriptor(typeof(IContentFactory), typeof(ContentFactory), ServiceLifetime.Singleton));
         services.Add(new ServiceDescriptor(typeof(IContentServiceFactory), typeof(ContentServiceFactory), ServiceLifetime.Singleton));
         //services.Add(new ServiceDescriptor(typeof(IDb), typeof(T), scope));

         return services;
      }
   }
}
