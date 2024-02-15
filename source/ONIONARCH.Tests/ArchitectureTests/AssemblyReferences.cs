using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Infrastructure.HealthChecks;
using ONIONARCH.Persistence.Contexts;
using ONIONARCH.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ONIONARCH.Tests.ArchitectureTests;

internal static class AssemblyReferences
{
    internal static readonly Assembly DomainAssembly = typeof(Entity).Assembly;
    internal static readonly Assembly ApplicationAssembly = typeof(IQueryDbContext).Assembly;
    internal static readonly Assembly InfrastrcutureAssembly = typeof(SimpleHealthCheck).Assembly;
    internal static readonly Assembly PersistenceAssembly = typeof(SampleQueryDbContext).Assembly;
    internal static readonly Assembly TestsAssembly = typeof(DomainArchitectureTests).Assembly;
}
