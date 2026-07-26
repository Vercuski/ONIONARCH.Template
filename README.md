![Onion Softare Architecture](./Documentation/Images/OnionArchitecture.png)

# Template Structure

## Domain Layer
- Third Party Libraries
  - None

## Application Layer
- Third Party Libraries
  - Dapper
  - MediatR
  - Microsoft.EntityFrameworkCore
  - Microsoft.Extensions.Hosting
  - Microsoft.IdentityModel.JsonWebTokens
  - System.IdentityModel.Tokens.Jwt

## Presentation Layer
### Presentation.API
- Third Party Libraries
  - Microsoft.AspNetCore.Authentication.JwtBearer
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
- Third Party Libraries
  - Microsoft.Data.SqlClient
  - Microsoft.EntityFrameworkCore.SqlServer
  - Npgsql
  - Npgsql.EntityFrameworkCore.PostgreSQL

  Both SQL Server and PostgreSQL packages are referenced because the database
  backend is swappable via `IDatabaseProvider` (`SqlServerDatabaseProvider` /
  `PostgreSqlDatabaseProvider`). The active provider is selected once, in each
  Presentation project's `Program.cs`.

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
  - FakeItEasy
  - Microsoft.NET.Test.Sdk
  - NetArchTest.Rules
  - NUnit
  - NUnit.Analyzers
  - NUnit3TestAdapter