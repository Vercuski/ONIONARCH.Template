using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Infrastructure.HealthChecks;
using ONIONARCH.Persistence.Contexts;
//#if (HasMySql)
using MySqlRegistration = ONIONARCH.Persistence.MySql.DependencyInjection;
//#endif
//#if (HasPostgreSql)
using PostgreSqlRegistration = ONIONARCH.Persistence.PostgreSql.DependencyInjection;
//#endif
//#if (HasSqlServer)
using SqlServerRegistration = ONIONARCH.Persistence.SqlServer.DependencyInjection;
//#endif
//#if (HasApi)
using ONIONARCH.Presentation.API.Controllers;
//#endif
using System.Reflection;

namespace ONIONARCH.Tests.ArchitectureTests;

/// <summary>
/// Assembly handles for each layer, resolved from a known type in that layer, shared by the
/// architecture fitness tests.
/// </summary>
internal static class AssemblyReferences
{
    /// <summary>The ONIONARCH.Domain assembly.</summary>
    internal static readonly Assembly DomainAssembly = typeof(Entity).Assembly;
    /// <summary>The ONIONARCH.Application assembly.</summary>
    internal static readonly Assembly ApplicationAssembly = typeof(IQueryDbContext).Assembly;
    /// <summary>The ONIONARCH.Infrastructure assembly.</summary>
    internal static readonly Assembly InfrastrcutureAssembly = typeof(SimpleHealthCheck).Assembly;
    /// <summary>The ONIONARCH.Persistence assembly.</summary>
    internal static readonly Assembly PersistenceAssembly = typeof(QueryDbContext).Assembly;
//#if (HasSqlServer)
    /// <summary>The ONIONARCH.Persistence.SqlServer assembly.</summary>
    internal static readonly Assembly SqlServerPersistenceAssembly = typeof(SqlServerRegistration).Assembly;
//#endif
//#if (HasPostgreSql)
    /// <summary>The ONIONARCH.Persistence.PostgreSql assembly.</summary>
    internal static readonly Assembly PostgreSqlPersistenceAssembly = typeof(PostgreSqlRegistration).Assembly;
//#endif
//#if (HasMySql)
    /// <summary>The ONIONARCH.Persistence.MySql assembly.</summary>
    internal static readonly Assembly MySqlPersistenceAssembly = typeof(MySqlRegistration).Assembly;
//#endif
    /// <summary>Every provider-specific persistence assembly.</summary>
    internal static readonly Assembly[] PersistenceProviderAssemblies =
    [
//#if (HasSqlServer)
        SqlServerPersistenceAssembly,
//#endif
//#if (HasPostgreSql)
        PostgreSqlPersistenceAssembly,
//#endif
//#if (HasMySql)
        MySqlPersistenceAssembly,
//#endif
    ];
//#if (HasApi)
    /// <summary>The ONIONARCH.Presentation.API assembly.</summary>
    internal static readonly Assembly PresentationAssembly = typeof(SampleController).Assembly;
//#endif
    /// <summary>The ONIONARCH.Tests assembly.</summary>
    internal static readonly Assembly TestsAssembly = typeof(DomainArchitectureTests).Assembly;
}
