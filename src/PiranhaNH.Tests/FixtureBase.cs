using System;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.ImageSharp;
using Piranha.Services;
using RimuTec.PiranhaNH.DataAccess;
using RimuTec.PiranhaNH.Repositories;
using RimuTec.PiranhaNH.Services;

namespace RimuTec.PiranhaNH
{
   public abstract class FixtureBase
   {
      protected IStorage _storage = new Piranha.Local.FileStorage("uploads/", "~/uploads/");
      protected IImageProcessor _processor = new ImageSharpProcessor();
      protected ICache _cache;

      protected IServiceProvider _services = CreateServiceCollection().BuildServiceProvider();
      protected ContentFactory _contentFactory;

      protected static IServiceCollection CreateServiceCollection()
      {
         return new ServiceCollection()
            .AddPiranhaNH()
            .AddPiranha()
            .AddPiranhaMemoryCache()
         //  .AddDistributedMemoryCache()
         ;
      }

      /// <summary>
      /// Creates a new api.
      /// </summary>
      protected virtual IApi CreateApi()
      {
         var factory = new ContentFactory(_services);
         var serviceFactory = new ContentServiceFactory(factory);

         var sessionFactory = Database.CreateSessionFactory();

         return new Api(
             factory,
             new AliasRepository(sessionFactory),
             null,//  new ArchiveRepository(sessionFactory),
             null,//  new MediaRepository(sessionFactory),
             new PageRepository(sessionFactory, serviceFactory, Module.Mapper),
             new PageTypeRepository(sessionFactory),
             new ParamRepository(sessionFactory),
             null,//  new PostRepository(sessionFactory, serviceFactory),
             new PostTypeRepository(sessionFactory),
             new SiteRepository(sessionFactory, serviceFactory),
             new SiteTypeRepository(sessionFactory),
             cache: _cache,
             storage: _storage,
             processor: _processor
         );
      }
   }
}
