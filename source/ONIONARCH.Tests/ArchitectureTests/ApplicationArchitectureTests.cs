using NetArchTest.Rules;
using ONIONARCH.Application.Abstractions;
using ONIONARCH.Tests.ArchitectureTests.CustomRules;
using static ONIONARCH.Tests.ArchitectureTests.AssemblyReferences;

namespace ONIONARCH.Tests.ArchitectureTests;

[TestFixture]
public class ApplicationArchitectureTests
{
    [Test]
    public void ApplicationEntityQueryHandlers_Should_HaveAnIQueryDbContextParameterInTheConstructor()
    {
        var customRuleIQueryDbContextMustBeConstructorParameter = new IQueryDbContextMustBeConstructorParameter();

        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceMatching("ONIONARCH.Application.Actions.*.Queries.*")
            .And()
            .ImplementInterface(typeof(IMediatRQueryHandler<,>))
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

    [Test]
    public void ApplicationEntityCommandHandlers_Should_HaveAnICommandDbContextParameterInTheConstructor()
    {
        var customRuleICommandDbContextMustBeConstructorParameter = new ICommandDbContextMustBeConstructorParameter();

        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceMatching("ONIONARCH.Application.Actions.*.Commands.*")
            .And()
            .ImplementInterface(typeof(IMediatRCommandHandler<,>))
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

    [Test]
    public void ApplicationAssembly_ShouldNot_ReferenceDapper()
    {
        // Application must depend only on abstractions (ICommandDbContext, IQueryDbContext,
        // ISampleEntityDapperQueryRepository, ISampleEntityDapperCommandRepository, etc.) that
        // are implemented in Persistence. This is a whole-assembly check, independent of the
        // constructor-shape rules above, so it also catches Dapper usage introduced outside a
        // MediatR handler (e.g. a helper class, static method, or future feature slice).
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

    [Test]
    public void ApplicationAssembly_ShouldNot_ReferenceEntityFrameworkCore()
    {
        // Mirrors ApplicationAssembly_ShouldNot_ReferenceDapper: EF Core is a Persistence-layer
        // implementation detail. Application should only see its own abstractions
        // (ICommandDbContext, IQueryDbContext, IUnitOfWork) and never the concrete EF Core types
        // those abstractions are backed by (EntityEntry<T>, IDbContextTransaction, DbContext
        // LINQ-async extensions, etc.).
        //
        // NOTE: as of this review this test is EXPECTED TO FAIL. Application currently has four
        // real references to Microsoft.EntityFrameworkCore:
        //   - Abstractions/Context/ICommandDbContext.cs   (EntityEntry<TEntity> return type)
        //   - Abstractions/IUnitOfWork.cs                 (IDbContextTransaction return type)
        //   - Actions/SampleEntityEFCore/Queries/GetMultipleSampleEntityEFCore.cs (ToListAsync)
        //   - Actions/SampleEntityEFCore/Queries/GetSingleSampleEntityEFCore.cs   (SingleOrDefaultAsync)
        // The first two are the tracked "ICommandDbContext leaks EF Core" architectural debt.
        // The latter two are EF Core's async LINQ extension methods, which have no
        // provider-agnostic equivalent without Application taking on its own async materialization
        // helper. This test is left enabled (rather than weakened) so the violation stays visible
        // until one of those is addressed.
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
