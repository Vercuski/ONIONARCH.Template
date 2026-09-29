using NetArchTest.Rules;
using ONIONARCH.Persistence.Providers;
using System.Reflection;
using static ONIONARCH.Tests.ArchitectureTests.AssemblyReferences;

namespace ONIONARCH.Tests.ArchitectureTests;

/// <summary>
/// Architecture fitness tests for the Persistence core and its provider-specific projects.
/// </summary>
[TestFixture]
public class PersistenceArchitectureTests
{
    /// <summary>
    /// Assemblies that only a provider-specific project may reference. Checked at the IL level
    /// (<see cref="Assembly.GetReferencedAssemblies"/>) rather than by namespace, because EF Core
    /// provider extension methods such as <c>UseSqlServer</c> and <c>UseNpgsql</c> live in the shared
    /// <c>Microsoft.EntityFrameworkCore</c> namespace and would be invisible to a namespace-based rule.
    /// </summary>
    private static readonly string[] ProviderSpecificAssemblyNames =
    [
        "Microsoft.EntityFrameworkCore.SqlServer",
        "Microsoft.Data.SqlClient",
        "Npgsql.EntityFrameworkCore.PostgreSQL",
        "Npgsql",
        "Microting.EntityFrameworkCore.MySql",
        "Pomelo.EntityFrameworkCore.MySql",
        "MySql.EntityFrameworkCore",
        "MySql.Data",
        "MySqlConnector"
    ];

//#if (HasMySql)
    /// <summary>
    /// Oracle's MySQL assemblies. The MySQL provider standardizes on MySqlConnector for both the EF Core
    /// and Dapper paths, so neither may appear alongside it.
    /// </summary>
    private static readonly string[] OracleMySqlAssemblyNames =
    [
        "MySql.EntityFrameworkCore",
        "MySql.Data"
    ];
//#endif

    /// <summary>
    /// Verifies that the Persistence core compiles against no provider-specific assembly, so any
    /// platform can be dropped without touching it.
    /// </summary>
    /// <remarks>
    /// The rule only sees references that survive into IL. A discarded <c>typeof(SqlConnection)</c>
    /// is elided by the compiler and is not detected; a real use of the type is.
    /// </remarks>
    [Test]
    public void PersistenceCore_ShouldNot_ReferenceProviderSpecificAssemblies()
    {
        var offending = ReferencedAssemblyNames(PersistenceAssembly)
            .Intersect(ProviderSpecificAssemblyNames, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Assert.That(offending, Is.Empty,
            $"ONIONARCH.Persistence must stay provider-agnostic but references: {string.Join(", ", offending)}");
    }

    /// <summary>
    /// Verifies that the Persistence core does not depend on any provider project; dependencies
    /// point from the providers to the core, never the reverse.
    /// </summary>
    [Test]
    public void PersistenceCore_ShouldNot_ReferenceProviderProjects()
    {
        var providerProjectNames = PersistenceProviderAssemblies.Select(a => a.GetName().Name!).ToList();

        var offending = ReferencedAssemblyNames(PersistenceAssembly)
            .Intersect(providerProjectNames, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Assert.That(offending, Is.Empty,
            $"ONIONARCH.Persistence must not depend on provider projects but references: {string.Join(", ", offending)}");
    }

    /// <summary>
    /// Verifies that a provider project depends on neither a sibling provider nor an outer layer,
    /// so each provider can be included or excluded independently.
    /// </summary>
    /// <param name="providerAssembly">The provider assembly under test.</param>
    [TestCaseSource(nameof(ProviderAssemblies))]
    public void ProviderProject_ShouldNot_ReferenceOtherProvidersOrOuterLayers(Assembly providerAssembly)
    {
        var forbidden = PersistenceProviderAssemblies
            .Where(a => a != providerAssembly)
            .Select(a => a.GetName().Name!)
            .Concat(["ONIONARCH.Presentation", "ONIONARCH.Infrastructure"])
            .ToArray();

        var result = Types
            .InAssembly(providerAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            $"{providerAssembly.GetName().Name} has forbidden dependencies: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

    /// <summary>
    /// Verifies that each provider project implements exactly one <see cref="IDatabaseProvider"/>.
    /// </summary>
    /// <param name="providerAssembly">The provider assembly under test.</param>
    [TestCaseSource(nameof(ProviderAssemblies))]
    public void ProviderProject_Should_ContainExactlyOneDatabaseProvider(Assembly providerAssembly)
    {
        var implementations = providerAssembly.GetTypes()
            .Where(t => typeof(IDatabaseProvider).IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false })
            .ToList();

        Assert.That(implementations, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Verifies that <see cref="IDatabaseProvider"/> implementations are internal and sealed. The
    /// registration extension (<c>AddSqlServer</c> etc.) is a provider project's only public surface,
    /// so hosts cannot construct a provider directly and bypass the registry.
    /// </summary>
    /// <param name="providerAssembly">The provider assembly under test.</param>
    [TestCaseSource(nameof(ProviderAssemblies))]
    public void DatabaseProviderImplementations_Should_BeInternalAndSealed(Assembly providerAssembly)
    {
        var result = Types
            .InAssembly(providerAssembly)
            .That()
            .ImplementInterface(typeof(IDatabaseProvider))
            .Should()
            .NotBePublic()
            .And()
            .BeSealed()
            .GetResult();

        Assert.That(result.IsSuccessful, Is.True,
            $"Provider types must be internal and sealed: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }

//#if (HasMySql)
    /// <summary>
    /// Verifies that the MySQL provider references no Oracle MySQL assembly, keeping the platform on
    /// a single ADO.NET driver (MySqlConnector).
    /// </summary>
    [Test]
    public void MySqlProvider_ShouldNot_ReferenceOracleMySqlAssemblies()
    {
        var offending = ReferencedAssemblyNames(MySqlPersistenceAssembly)
            .Intersect(OracleMySqlAssemblyNames, StringComparer.OrdinalIgnoreCase)
            .ToList();

        Assert.That(offending, Is.Empty,
            $"ONIONARCH.Persistence.MySql must use MySqlConnector only but references: {string.Join(", ", offending)}");
    }
//#endif

    /// <summary>
    /// Supplies each provider assembly as a named test case.
    /// </summary>
    /// <returns>One test case per provider assembly.</returns>
    private static IEnumerable<TestCaseData> ProviderAssemblies() =>
        PersistenceProviderAssemblies.Select(a => new TestCaseData(a).SetArgDisplayNames(a.GetName().Name!));

    /// <summary>
    /// Returns the simple names of the assemblies <paramref name="assembly"/> references in IL.
    /// </summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>The referenced assembly names.</returns>
    private static IEnumerable<string> ReferencedAssemblyNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(a => a.Name!);
}
