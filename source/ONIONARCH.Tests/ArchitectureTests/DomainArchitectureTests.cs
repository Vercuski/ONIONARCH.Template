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
    /// Verifies that the Domain assembly does not depend on any outer layer.
    /// </summary>
    /// <remarks>
    /// <c>HaveDependencyOnAll</c> only flags a type that references <em>every</em> listed namespace,
    /// so a single stray reference to one layer is not detected; <c>HaveDependencyOnAny</c> would
    /// enforce the intent strictly.
    /// </remarks>
    [Test]
    public void DomainAssembly_ShouldNot_ReferenceAnyOtherProjects()
    {
        var result = Types
            .InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAll([
                "Application",
                "Infrastructure",
                "Persistence",
                "Presentation",
                "Tests"
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
