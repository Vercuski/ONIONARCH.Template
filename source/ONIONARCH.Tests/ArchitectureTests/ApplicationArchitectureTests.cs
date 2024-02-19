using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Infrastructure.HealthChecks;
using ONIONARCH.Persistence.Contexts;
using ONIONARCH.Presentation.Web;
using ONIONARCH.Tests.ArchitectureTests.CustomRules;
using FluentAssertions;
using NetArchTest.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
            .ResideInNamespaceMatching("ONIONARCH.Application.Entities.*.Queries.*")
            .And()
            .ImplementInterface(typeof(IQueryHandler<,>))
            .Should()
            .MeetCustomRule(customRuleIQueryDbContextMustBeConstructorParameter)
            .And()
            .BeSealed()
            .GetResult();

        if (result.FailingTypeNames != null && result.FailingTypeNames.Any()) {
            Console.WriteLine("Failing Entity Types:");
            foreach (var failingType in result.FailingTypeNames)
            {
                Console.WriteLine($"    {failingType}");
            }
        }
        result.IsSuccessful.Should().BeTrue();
    }

    [Test]
    public void ApplicationEntityCommandHandlers_Should_HaveAnICommandDbContextParameterInTheConstructor()
    {
        var customRuleICommandDbContextMustBeConstructorParameter = new ICommandDbContextMustBeConstructorParameter();

        var result = Types
            .InAssembly(ApplicationAssembly)
            .That()
            .ResideInNamespaceMatching("ONIONARCH.Application.Entities.*.Commands.*")
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
        result.IsSuccessful.Should().BeTrue();
    }
}
