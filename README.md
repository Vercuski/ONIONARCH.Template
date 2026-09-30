![Onion Softare Architecture](./Documentation/Images/OnionArchitecture.png)

# Template Structure

## Domain Layer
- Third Party Libraries
  - None

## Application Layer
- Third Party Libraries
  - Microsoft.EntityFrameworkCore
  - Microsoft.Extensions.Hosting

  CQRS dispatch (`ISender`, request/handler contracts, pipeline behaviors) is a small in-house
  implementation in `Application/Sender.cs` and `Application/Abstractions/`, rather than a
  third-party mediator library — see `IRequestHandler<,>`, `IQueryHandler<,>`,
  `ICommandHandler<,>`, and `IPipelineBehavior<,>`.

## Presentation Layer
### Presentation.API
- Third Party Libraries
  - Microsoft.AspNetCore.OpenApi
  - Microsoft.OpenApi
  - Microsoft.VisualStudio.Azure.Containers.Tools.Targets
  - Scalar.AspNetCore
  
### Presentation.Console
- Third Party Libraries
  - Microsoft.Extensions.Hosting
  - Spectre.Console
  - Spectre.Console.Cli

### Presentation.Web
- Third Party Libraries
  - Microsoft.VisualStudio.Azure.Containers.Tools.Targets
  - MudBlazor
  
## Persistence Layer
Persistence is split into a provider-agnostic core and one project per database platform.
The core never references a concrete provider; each host opts providers in at its
composition root.

### Persistence (core)
- Third Party Libraries
  - Dapper
  - EFCore.BulkExtensions.Core (dual licensed, see [Bulk operations](#bulk-operations-ef-core))
  - Microsoft.EntityFrameworkCore.Relational
  - Microsoft.EntityFrameworkCore.Design

  Owns the DbContexts, Dapper repositories, connection factories, the `IDatabaseProvider`
  port and the `DatabaseProviderRegistry`. The active provider is selected per
  query/command side via `DatabasePlatformOptions` in each Presentation project's
  `appsettings.json` and resolved from the registry at startup; an unregistered
  platform fails fast with the list of registered ones.

### Persistence.SqlServer / Persistence.PostgreSql / Persistence.MySql
| Project | Platform key | Third Party Libraries |
|---|---|---|
| `Persistence.SqlServer` | `MSSQL` | Microsoft.EntityFrameworkCore.SqlServer, Microsoft.Data.SqlClient, EFCore.BulkExtensions.SqlServer |
| `Persistence.PostgreSql` | `PostgreSQL` | Npgsql.EntityFrameworkCore.PostgreSQL, Npgsql, EFCore.BulkExtensions.PostgreSql |
| `Persistence.MySql` | `MySQL` | Microting.EntityFrameworkCore.MySql, MySqlConnector |

  Each project exposes a single public registration extension; its `IDatabaseProvider`
  implementation is internal. Hosts register every platform either side may be configured for:

```csharp
builder.AddPersistenceRegistrations(providers =>
{
    providers.AddSqlServer();
    providers.AddPostgreSql();
    providers.AddMySql(builder.Configuration);
});
```

  MySQL (and MariaDB) uses `Microting.EntityFrameworkCore.MySql`, a community fork of
  Pomelo.EntityFrameworkCore.MySql that tracks EF Core 10; Pomelo itself has no EF Core 10 release
  yet. It is built on MySqlConnector, so the EF Core and Dapper paths share one ADO.NET driver.
  EF Core targets the server version in `DatabasePlatform:MySqlServerVersion` (e.g. `8.4.0-mysql`
  or `11.4.2-mariadb`) rather than auto-detecting it, which would open a connection just to
  configure a DbContext. The value is validated at startup whenever `AddMySql` is registered.

  To drop a platform, remove its `Add…()` call and its project reference from each host.
  `PersistenceArchitectureTests` enforce that the core stays provider-agnostic and that
  provider projects don't reference each other.

### Bulk operations (EF Core)
`IBulkCommandDbContext` (Application) is the write-side port for bulk work that should skip the
change tracker. `Persistence/Bulk/EfCoreBulkCommandDbContext` implements it over the same scoped
`CommandDbContext`, so every call uses the command connection and joins a transaction started with
`IUnitOfWork.BeginTransactionAsync`.

| Method | Implementation | SQL Server | PostgreSQL | MySQL |
|---|---|:-:|:-:|:-:|
| `BulkInsertAsync(entities, retrieveGeneratedKeys)` | [EFCore.BulkExtensions](https://github.com/borisdj/EFCore.BulkExtensions) `BulkInsert` | ✓ | ✓ | ✗ |
| `BulkUpdateAsync(entities)` (match by key) | EFCore.BulkExtensions `BulkUpdate` | ✓ | ✓ | ✗ |
| `BulkDeleteAsync(entities)` (match by key) | EFCore.BulkExtensions `BulkDelete` | ✓ | ✓ | ✗ |
| `BulkUpsertAsync(entities, retrieveGeneratedKeys)` | EFCore.BulkExtensions `BulkInsertOrUpdate` | ✓ | ✓ | ✗ |
| `UpdateWhereAsync(predicate, set => set.Set(...))` | EF Core `ExecuteUpdateAsync` | ✓ | ✓ | ✓ |
| `DeleteWhereAsync(predicate)` | EF Core `ExecuteDeleteAsync` | ✓ | ✓ | ✓ |

```csharp
await bulk.UpdateWhereAsync<Order>(
    o => o.Status == OrderStatus.Pending && o.CreatedUtc < cutoff,
    set => set
        .Set(o => o.Status, OrderStatus.Expired)
        .Set(o => o.RetryCount, o => o.RetryCount + 1),
    cancellationToken);
```

  The core project uses only the provider-agnostic `EFCore.BulkExtensions.Core`. Each provider project
  references its own platform adapter, which the library finds at runtime, and reports support through
  `IDatabaseProvider.SupportsBulkOperations`. `PersistenceArchitectureTests` keep the adapters out of
  the core. Things to know:
  - **Licensing.** EFCore.BulkExtensions is dual licensed. It is free for open source software,
    non-profits, and companies with under USD 1M annual gross revenue. Anyone else needs a
    [commercial license](https://codis.tech/efcorebulk). A solution generated from this template
    carries that dependency, so check the terms for your organization.
  - **MySQL.** EFCore.BulkExtensions has no EF Core 10 adapter for MySQL. `EFCore.BulkExtensions.MySql`
    stops at 9.x, is built on Pomelo/EF Core 9, and conflicts with the Microting EF Core 10 provider.
    When the command database is MySQL, the entity-list methods throw `NotSupportedException`
    (so do the sample `POST`/`PUT api/Sample/EFCore/Bulk` endpoints, which return 500).
    `UpdateWhereAsync` and `DeleteWhereAsync` still work. Revisit when an EF Core 10 adapter ships.
  - **Change tracker.** Bulk calls run immediately. Tracked entities are not refreshed, and
    concurrency tokens are not checked. Don't mix tracked and bulk writes to the same rows.
  - **Upsert with generated keys.** If an entity's store-generated key is still unset (e.g. `0`),
    `BulkUpsertAsync` inserts it with `BulkInsert`. The rest go through `BulkInsertOrUpdate`, and both
    steps run in one transaction. Sending unsaved entities straight to `BulkInsertOrUpdate` isn't
    portable: PostgreSQL's adapter writes the `0` literally.
  - **Generated keys.** Returning them (`retrieveGeneratedKeys: true`) adds a staging step. On SQL
    Server it made a 100k-row insert about 3x slower, so ask for keys only when you need them.
  - **One tuning point.** `EfCoreBulkCommandDbContext.CreateConfig` builds the `BulkConfig` for every
    call. Batch size, timeouts, and SQL Server bulk-copy options belong there.

  The sample exposes `POST` (insert, returns keys), `PUT` (upsert), `PATCH` (update-where) and
  `DELETE` (delete-where) on `api/Sample/EFCore/Bulk`.

## Infrastructure Layer
- Third Party Libraries
  - Azure.Identity
- Framework References
  - Microsoft.AspNetCore.App (needed for `GlobalExceptionHandler` and the
    health check response writer, both of which are ASP.NET Core-specific
    cross-cutting concerns centralized in this layer)

## Testing Layer
- Third Party Libraries
  - coverlet.collector
  - EFCore.BulkExtensions.Sqlite, Microsoft.EntityFrameworkCore.Sqlite.Core, SQLitePCLRaw.bundle_e_sqlite3
    (in-memory database for the bulk-operation tests. The SQLite bundle is pinned past the one EF Core
    10.0.10 references, which GHSA-2m69-gcr7-jv3q flags)
  - FakeItEasy
  - Microsoft.NET.Test.Sdk
  - NetArchTest.Rules
  - NUnit
  - NUnit.Analyzers
  - NUnit3TestAdapter

# Versioning

The solution follows [Semantic Versioning 2.0](https://semver.org). Versions are calculated at
build time by [MinVer](https://github.com/adamralph/minver) from git tags, so there are no version
numbers to edit in any project file. The configuration lives in `source/Directory.Build.props`, and
every project gets the same version.

| Git state | Version |
|---|---|
| No tags yet | `1.0.0-alpha.0.{height}` |
| Commit tagged `v1.4.0` | `1.4.0` |
| 3 commits after `v1.4.0` | `1.4.1-alpha.0.3` |
| Commit tagged `v1.5.0-rc.1` | `1.5.0-rc.1` |
| 2 commits after `v1.5.0-rc.1` | `1.5.0-rc.1.2` |

`{height}` is the number of commits since the last tag, so every build has a unique version that
sorts correctly. The assembly attributes are set as follows:

- `AssemblyInformationalVersion` is the full version plus build metadata (the commit SHA), e.g.
  `1.4.1-alpha.0.3+2057147a69…`.
- `FileVersion` is `{Major}.{Minor}.{Patch}.0`.
- `AssemblyVersion` is `{Major}.0.0.0`, so it only changes on a breaking (major) release.

## Cutting a release

```bash
git tag v1.4.0          # or a pre-release: git tag v1.5.0-rc.1
git push origin v1.4.0
```

Choose the number by the SemVer rules: MAJOR for breaking changes, MINOR for backward-compatible
features, PATCH for backward-compatible fixes. Tags must start with `v`.

## Where the version appears

- The `/health` response includes `version` and `informationalVersion`.
- The API's OpenAPI document (`/openapi/v1.json`, shown in Scalar) uses the version as `info.version`.
- Every host (API, Web, Console) logs `Starting {Application} {Version} … in {Environment}` at startup.
- In code, inject `ONIONARCH.Infrastructure.Versioning.ApplicationVersion` (a singleton) or use
  `ApplicationVersion.Current`.

## CI and Docker

- The GitHub Actions workflow (`.github/workflows/build.yml`) checks out with `fetch-depth: 0`. MinVer
  needs the tags and history, and a shallow clone would make every build look untagged.
- Docker builds have no `.git` in their context, so the version is passed in as a build argument.
  Without it the image is built as `1.0.0-alpha.0` and MinVer logs warning `MINVER1001`.

  ```bash
  docker build source -f source/ONIONARCH.Presentation.API/Dockerfile --build-arg APP_VERSION=1.4.0
  ```

  The image also gets an `org.opencontainers.image.version` label.
- Any other build can force a version with `-p:MinVerVersionOverride=1.4.0`.
# Using as a `dotnet new` Template
The solution in `source/` doubles as a `dotnet new` template (`source/.template.config/`), packaged by
`template/Vercuski.OnionArch.Templates.csproj`. Installed templates also appear in Visual Studio's
New Project dialog, where the options below show as checkboxes and a drop-down.

```
dotnet new install source/                    # from a clone, or:
dotnet pack template/Vercuski.OnionArch.Templates.csproj -c Release -o artifacts
dotnet new install artifacts/Vercuski.OnionArch.Templates.<version>.nupkg

dotnet new onionarch -n Acme.Orders --web true --database PostgreSql
```

`-n` replaces `ONIONARCH` everywhere: project and solution names, namespaces, `ProjectKey`, and the
SonarQube project key. Solution GUIDs, `UserSecretsId`s, and development ports are regenerated.

| Option | Values | Default | Controls |
|---|---|---|---|
| `--api` | bool | `true` | Presentation.API host, its architecture and integration tests |
| `--web` | bool | `false` | Presentation.Web host (Blazor + MudBlazor) |
| `--console` | bool | `false` | Presentation.Console worker host |
| `--database`, `-db` | `SqlServer`, `PostgreSql`, `MySql`, `All` | `SqlServer` | Provider projects, their registrations, Dockerfile lines, and the `DatabasePlatform` settings |
| `--data-access`, `-da` | `EfCore`, `Dapper`, `Both` | `Both` | The EF Core path (DbContexts, unit of work), the Dapper path (connection factories, repository ports), their provider methods and packages, and the MySQL server-version setting (EF only) |
| `--samples` | bool | `true` | The `SampleEntity` slice: entity, actions for each included path, DTOs, Dapper repositories, `SampleController` |
| `--tests` | bool | `true` | The test project |
| `--docker` | bool | `true` | Dockerfiles, `.dockerignore`, container launch profiles and tooling |
| `--quality-tooling` | bool | `false` | `SonarqubeAnalysis.bat`, `coverConfig.xml` |

If no host is selected, the API is included. `--database All` reproduces this repository's full
provider set and leaves the per-side choice to `appsettings.json`.

## Authoring rules
- The repository stays the reference implementation: every template directive is a comment
  (`//#if`, `<!--#if -->`, `#if` in Dockerfiles), so `source/ONIONARCH.sln` builds and tests as-is.
- The generated solution file comes from `source/.template.config/solution/ONIONARCH.App.sln`, a
  copy of the real solution with conditional blocks, kept separate because Visual Studio rewrites
  `ONIONARCH.sln` on save. When you add or remove a project, update both. Its name deliberately
  differs from `sourceName`: Visual Studio opens `<Name>.App.sln` (found through `primaryOutputs`
  and the `editorTreatAs: solution` tag) and re-saves it under the name chosen in the New Project
  dialog, while `dotnet new` renames it to `<Name>.sln`. A solution named after `sourceName` makes
  Visual Studio stop on a "File Modification Detected" prompt right after creation.
- Where a variant needs *different* code rather than less code, the alternate lives in a `//#else`
  branch on lines prefixed `//~`. They stay comments in the repository and are uncommented in
  generated output (see `SpecialCustomOperations` in `template.json`; the replacement operation must
  not be given an `id`, which leaves it disabled).
- `template/verify-template.sh` generates one option combination and checks that it builds with zero
  warnings, passes its tests, contains no template directives or `ONIONARCH`/`Vercuski` leftovers,
  has valid JSON and database settings that match the generated providers, and that its `.sln`
  matches the projects on disk. The `Template` workflow runs it across a matrix of combinations;
  its all-on case also fails if the template solution drifts from `source/ONIONARCH.sln`.

# Documentation

API documentation is generated as Markdown from the XML documentation comments (`///`) with
[DocFX](https://dotnet.github.io/docfx/). The configuration lives in `Documentation/DocFX`.

- On every push to `main`, the `Documentation` workflow (`.github/workflows/docs.yml`) generates the
  pages and publishes them to [Vercuski/RepoDocumentation](https://github.com/Vercuski/RepoDocumentation)
  under a folder named after this repository (`ONIONARCH.Template/`), where GitHub renders them
  directly: a `README.md` landing page (from `Documentation/DocFX/landing-page.md`) and one page per
  namespace and type under `api/`.
- DocFX's Markdown output leaves `<see cref="..."/>` references unresolved, so
  `Documentation/DocFX/PostProcess.cs` (a .NET 10 file-based app) turns them into links. It fails the
  build on any broken link or anchor, and DocFX runs with `--warningsAsErrors`, matching
  `TreatWarningsAsErrors` for code.
- Publishing requires a `DOCS_REPO_TOKEN` secret: a fine-grained personal access token with
  **Contents: Read and write** on `Vercuski/RepoDocumentation` only.
- To generate the same output locally (written to `Documentation/DocFX/_site`):

  ```bash
  dotnet tool install --global docfx
  cd Documentation/DocFX
  docfx metadata docfx.json
  dotnet run PostProcess.cs
  ```
