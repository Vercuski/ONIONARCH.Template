using NetArchTest.Rules;
using static ONIONARCH.Tests.ArchitectureTests.AssemblyReferences;

namespace ONIONARCH.Tests.ArchitectureTests;

/// <summary>
/// Architecture fitness tests for the API presentation layer, scoped to its controllers.
/// </summary>
[TestFixture]
public class PresentationArchitectureTests
{
    /// <summary>The namespace containing the API controllers under test.</summary>
    private const string ControllersNamespace = "ONIONARCH.Presentation.API.Controllers";

    // Scoped to the Controllers namespace rather than the whole assembly: Program.cs is the
    // composition root and legitimately calls AddPersistenceRegistrations()
    // to wire up DI, so an assembly-wide ban on referencing Persistence would fail for the wrong
    // reason. Controllers, on the other hand, have no legitimate reason to see Persistence at all —
    // they should only talk to Application via ISender and get DTOs back.
    /// <summary>
    /// Verifies that no controller depends on the Persistence layer.
    /// </summary>
    [Test]
    public void Controllers_ShouldNot_ReferencePersistenceDirectly()
    {
        var result = Types
            .InAssembly(PresentationAssembly)
            .That()
            .ResideInNamespace(ControllersNamespace)
            .ShouldNot()
            .HaveDependencyOn("ONIONARCH.Persistence")
            .GetResult();

        if (result.FailingTypeNames != null && result.FailingTypeNames.Any())
        {
            Console.WriteLine("Controllers Referencing Persistence:");
            foreach (var failingType in result.FailingTypeNames)
            {
                Console.WriteLine($"    {failingType}");
            }
        }
        Assert.That(result.IsSuccessful, Is.True);
    }

    /// <summary>
    /// Verifies that no controller depends on Dapper.
    /// </summary>
    [Test]
    public void Controllers_ShouldNot_ReferenceDapperDirectly()
    {
        // Same rationale as ApplicationAssembly_ShouldNot_ReferenceDapper: raw SQL access belongs
        // behind a repository abstraction in Persistence, invoked through Application/ISender —
        // never directly in a controller.
        var result = Types
            .InAssembly(PresentationAssembly)
            .That()
            .ResideInNamespace(ControllersNamespace)
            .ShouldNot()
            .HaveDependencyOn("Dapper")
            .GetResult();

        if (result.FailingTypeNames != null && result.FailingTypeNames.Any())
        {
            Console.WriteLine("Controllers Referencing Dapper:");
            foreach (var failingType in result.FailingTypeNames)
            {
                Console.WriteLine($"    {failingType}");
            }
        }
        Assert.That(result.IsSuccessful, Is.True);
    }
}
