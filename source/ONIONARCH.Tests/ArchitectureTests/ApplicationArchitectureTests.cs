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
    /// (see <see cref="IQueryDbContextMustBeConstructorParameter"/>).
    /// </summary>
    [Test]
    public void ApplicationEntityQueryHandlers_Should_HaveAnIQueryDbContextParameterInTheConstructor()
    {
        var customRuleIQueryDbContextMustBeConstructorParameter = new IQueryDbContextMustBeConstructorParameter();

        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceMatching("ONIONARCH.Application.Actions.*.Queries.*")
            .And()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .MeetCustomRule(customRuleIQueryDbContextMustBeConstructorParameter)
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
    /// (see <see cref="ICommandDbContextMustBeConstructorParameter"/>).
    /// </summary>
    [Test]
    public void ApplicationEntityCommandHandlers_Should_HaveAnICommandDbContextParameterInTheConstructor()
    {
        var customRuleICommandDbContextMustBeConstructorParameter = new ICommandDbContextMustBeConstructorParameter();

        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceMatching("ONIONARCH.Application.Actions.*.Commands.*")
            .And()
            .ImplementInterface(typeof(ICommandHandler<,>))
            .Should()
            .MeetCustomRule(customRuleICommandDbContextMustBeConstructorParameter)
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
        // Application must depend only on abstractions (ICommandDbContext, IQueryDbContext,
        // ISampleEntityDapperQueryRepository, ISampleEntityDapperCommandRepository, etc.) that
        // are implemented in Persistence. This is a whole-assembly check, independent of the
        // constructor-shape rules above, so it also catches Dapper usage introduced outside a
        // request handler (e.g. a helper class, static method, or future feature slice).
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
    /// persistence ports (<c>IQueryDbContext</c>, <c>ICommandDbContext</c>) free of EF Core types.
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
