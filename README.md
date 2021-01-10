# About RimuTec.Piranha

This repository contains additional modules for Piranha CMS (https://github.com/PiranhaCMS/piranha.core)

The first module is RimuTec.PiranhaNH which uses NHibernate (NH) instead of Entity Framework Core (EF Core). The module is currently in development.

# Approach

The first step is to implement 10 repositories. Here is a list included where we believe that we have an initial implementation:

- [x] AliasRepository
- [ ] ArchiveRepository
- [ ] MediaRepository
- [ ] PageRepository
- [x] PageTypeRepository
- [x] ParamRepository
- [ ] PostRepository
- [x] PostTypeRepository
- [ ] SiteRepository; missing:
  - [ ] GetSiteMap()
- [x] SiteTypeRepository

We are using a test-driven approach. That means we are using tests to drive the implementation of each repository.

Once we have the initial implementation of these repositories working, we can then use the set of integration tests for the services in Piranha.Core:
- AliasService
- ArchiveService
- MediaService
- PageService
- PageTypeService
- ParamService
- PostService
- PostTypeService
- SiteService
- SiteTypeService
