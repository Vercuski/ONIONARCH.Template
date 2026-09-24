using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Infrastructure.HealthChecks;
using ONIONARCH.Persistence.Contexts;
using ONIONARCH.Presentation.API.Controllers;
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
    /// <summary>The ONIONARCH.Presentation.API assembly.</summary>
    internal static readonly Assembly PresentationAssembly = typeof(SampleController).Assembly;
    /// <summary>The ONIONARCH.Tests assembly.</summary>
    internal static readonly Assembly TestsAssembly = typeof(DomainArchitectureTests).Assembly;
}
