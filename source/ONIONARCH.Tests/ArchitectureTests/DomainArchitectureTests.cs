using NetArchTest.Rules;
using ONIONARCH.Domain.Abstractions;
using static ONIONARCH.Tests.ArchitectureTests.AssemblyReferences;

namespace ONIONARCH.Tests.ArchitectureTests;

/// <summary>
/// Architecture fitness tests for the Domain layer, the innermost ring of the onion.
/// </summary>
[TestFixture]
public class DomainArchitectureTests
{
    /// <summary>
    /// Verifies that every type in <c>ONIONARCH.Domain.Entities</c> inherits from
    /// <see cref="Entity"/> and is sealed.
    /// </summary>
    [Test]
    public void DomainEntities_Should_InheritFromTheEntityTypeAndBeSealed()
    {
        var result = Types
            .InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("ONIONARCH.Domain.Entities")
            .Should()
            .Inherit(typeof(Entity))
            .And()
            .BeSealed()
            .GetResult();

        if (result.FailingTypeNames != null && result.FailingTypeNames.Any())
        {
            Console.WriteLine("Failing Entity Types:");
            foreach (var failingType in result.FailingTypeNames)
            {
                Console.WriteLine($"    {failingType}");
            }
        }
        Assert.That(result.IsSuccessful, Is.True);
    }

    /// <summary>
    /// Verifies that no type in the Domain assembly depends on any outer layer or on the tests.
    /// </summary>
    /// <remarks>
    /// Uses <c>HaveDependencyOnAny</c>, so a single reference to any one listed namespace fails the
    /// test. (<c>HaveDependencyOnAll</c> would only flag a type referencing every listed namespace at
    /// once.) NetArchTest matches dependencies by namespace prefix, so the names must be fully
    /// qualified: an unqualified <c>"Application"</c> never matches <c>ONIONARCH.Application</c>.
    /// The <c>ONIONARCH.Persistence</c> prefix also covers the provider-specific projects.
    /// </remarks>
    [Test]
    public void DomainAssembly_ShouldNot_ReferenceAnyOtherProjects()
    {
        var result = Types
            .InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny([
                "ONIONARCH.Application",
                "ONIONARCH.Infrastructure",
                "ONIONARCH.Persistence",
                "ONIONARCH.Presentation",
                "ONIONARCH.Tests"
            ])
            .GetResult();

        if (result.FailingTypeNames != null && result.FailingTypeNames.Any())
        {
            Console.WriteLine("Failing Reference Types:");
            foreach (var failingType in result.FailingTypeNames)
            {
                Console.WriteLine($"    {failingType}");
            }
        }
        Assert.That(result.IsSuccessful, Is.True);
    }

    /// <summary>
    /// Verifies that every type in <c>ONIONARCH.Domain.Options</c> implements
    /// <see cref="IBaseOptionsConfig"/> and is sealed.
    /// </summary>
    /// <remarks>
    /// The options types currently live in <c>ONIONARCH.Persistence.Options</c>, so this test matches
    /// no types and passes vacuously.
    /// </remarks>
    [Test]
    public void OptionsEntities_Should_InheritFromTheBaseConfigTypeAndBeSealed()
    {
        var result = Types
            .InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("ONIONARCH.Domain.Options")
            .Should()
            .ImplementInterface(typeof(IBaseOptionsConfig))
            .And()
            .BeSealed()
            .GetResult();

        if (result.FailingTypeNames != null && result.FailingTypeNames.Any())
        {
            Console.WriteLine("Failing Options Types:");
            foreach (var failingType in result.FailingTypeNames)
            {
                Console.WriteLine($"    {failingType}");
            }
        }
        Assert.That(result.IsSuccessful, Is.True);
    }
}
