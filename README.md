# About RimuTec.Piranha

This repository contains additional modules for Piranha CMS (https://github.com/PiranhaCMS/piranha.core)

The first module is RimuTec.PiranhaNH which uses NHibernate (NH) instead of Entity Framework Core (EF Core). The module is currently in development.

# Approach

The first step is to implement 10 repository classes that Piranha.Core uses. Here is a list of them along with an indication which ones have their initial implementation::

- AliasRepository ✔️
- ArchiveRepository ❌
- MediaRepository ❌
- PageRepository 🔧
- PageTypeRepository ✔️
- ParamRepository ✔️
- PostRepository ❌
- PostTypeRepository ✔️
- SiteRepository ✔️
- SiteTypeRepository ✔️

✔️ = complete, 🔧 = in progress, ❌ = to do

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
