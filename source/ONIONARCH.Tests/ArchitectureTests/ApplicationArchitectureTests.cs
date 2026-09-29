using NetArchTest.Rules;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Tests.ArchitectureTests.CustomRules;
using static ONIONARCH.Tests.ArchitectureTests.AssemblyReferences;

namespace ONIONARCH.Tests.ArchitectureTests;

/// <summary>
/// Architecture fitness tests for the Application layer: handler shape rules and the ban on
/// referencing persistence technologies (Dapper, EF Core) directly.
/// </summary>
[TestFixture]
public class ApplicationArchitectureTests
{
    /// <summary>
    /// Verifies that every query handler under <c>Actions.*.Queries</c> is sealed and takes a
    /// query-side persistence abstraction in its constructor
    /// (see <see cref="QueryHandlerMustDependOnQueryPort"/>).
    /// </summary>
    [Test]
    public void ApplicationEntityQueryHandlers_Should_DependOnAQuerySidePort()
    {
        var queryPortRule = new QueryHandlerMustDependOnQueryPort();

        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceMatching("ONIONARCH.Application.Actions.*.Queries.*")
            .And()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .MeetCustomRule(queryPortRule)
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
    /// Verifies that every command handler under <c>Actions.*.Commands</c> is sealed and takes a
    /// command-side persistence abstraction in its constructor
    /// (see <see cref="CommandHandlerMustDependOnCommandPort"/>).
    /// </summary>
    [Test]
    public void ApplicationEntityCommandHandlers_Should_DependOnACommandSidePort()
    {
        var commandPortRule = new CommandHandlerMustDependOnCommandPort();

        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceMatching("ONIONARCH.Application.Actions.*.Commands.*")
            .And()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .MeetCustomRule(commandPortRule)
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
    /// Verifies that no type in the Application assembly depends on Dapper.
    /// </summary>
    [Test]
    public void ApplicationAssembly_ShouldNot_ReferenceDapper()
    {
        // Application must depend only on persistence ports (the EF Core contexts and/or the Dapper
        // repository ports) that are implemented in Persistence. This is a whole-assembly check,
        // independent of the constructor-shape rules above, so it also catches Dapper usage introduced
        // outside a request handler (e.g. a helper class, static method, or future feature slice).
        var result = Types
            .InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Dapper")
            .GetResult();

        if (result.FailingTypeNames != null && result.FailingTypeNames.Any())
        {
            Console.WriteLine("Types Referencing Dapper:");
            foreach (var failingType in result.FailingTypeNames)
            {
                Console.WriteLine($"    {failingType}");
            }
        }
        Assert.That(result.IsSuccessful, Is.True);
    }

    /// <summary>
    /// Verifies that no type in the Application assembly depends on EF Core, keeping the
    /// persistence ports free of EF Core types.
    /// </summary>
    [Test]
    public void ApplicationAssembly_ShouldNot_ReferenceEntityFrameworkCore()
    {
        var result = Types
            .InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("Microsoft.EntityFrameworkCore")
            .GetResult();

        if (result.FailingTypeNames != null && result.FailingTypeNames.Any())
        {
            Console.WriteLine("Types Referencing Microsoft.EntityFrameworkCore:");
            foreach (var failingType in result.FailingTypeNames)
            {
                Console.WriteLine($"    {failingType}");
            }
        }
        Assert.That(result.IsSuccessful, Is.True);
    }
}
