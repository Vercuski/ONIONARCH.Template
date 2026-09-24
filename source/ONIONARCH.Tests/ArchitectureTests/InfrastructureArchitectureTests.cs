using NetArchTest.Rules;
using static ONIONARCH.Tests.ArchitectureTests.AssemblyReferences;

namespace ONIONARCH.Tests.ArchitectureTests;

/// <summary>
/// Architecture fitness tests for the Infrastructure layer.
/// </summary>
[TestFixture]
public class InfrastructureArchitectureTests
{
    // Infrastructure currently has zero ProjectReferences — it only depends on the ASP.NET Core
    // shared framework and Azure.Identity. This test guards that invariant: nothing in Infrastructure
    // should ever need to see Application, Domain, Persistence, or Presentation types. If it does,
    // that's a sign the new code belongs somewhere else (most likely as a port in Application,
    // implemented in whichever layer actually needs the dependency).
    /// <summary>
    /// Verifies that the Infrastructure assembly does not depend on the Application, Domain,
    /// Persistence, or Presentation layers.
    /// </summary>
    /// <remarks>
    /// <c>HaveDependencyOnAll</c> only flags a type that references <em>every</em> listed namespace,
    /// so a single stray reference to one layer is not detected; <c>HaveDependencyOnAny</c> would
    /// enforce the intent strictly.
    /// </remarks>
    [Test]
    public void InfrastructureAssembly_ShouldNot_ReferenceApplicationDomainPersistenceOrPresentation()
    {
        var result = Types
            .InAssembly(InfrastrcutureAssembly)
            .ShouldNot()
            .HaveDependencyOnAll([
                "ONIONARCH.Application",
                "ONIONARCH.Domain",
                "ONIONARCH.Persistence",
                "ONIONARCH.Presentation"
            ])
            .GetResult();

        if (result.FailingTypeNames != null && result.FailingTypeNames.Any())
        {
            Console.WriteLine("Infrastructure Types Referencing Other Layers:");
            foreach (var failingType in result.FailingTypeNames)
            {
                Console.WriteLine($"    {failingType}");
            }
        }
        Assert.That(result.IsSuccessful, Is.True);
    }
}
