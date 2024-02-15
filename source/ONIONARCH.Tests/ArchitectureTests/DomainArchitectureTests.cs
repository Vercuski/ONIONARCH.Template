using ONIONARCH.Application.Abstractions;
using ONIONARCH.Domain.Abstractions;
using ONIONARCH.Infrastructure.HealthChecks;
using ONIONARCH.Persistence.Contexts;
using ONIONARCH.Presentation;
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
public class DomainArchitectureTests
{
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
        result.IsSuccessful.Should().BeTrue();
    }

    [Test]
    public void OptionsEntities_Should_InheritFromTheBaseConfigTypeAndBeSealed()
    {
        var result = Types
            .InAssembly(DomainAssembly)
            .That()
            .ResideInNamespace("ONIONARCH.Domain.Options")
            .Should()
            .Inherit(typeof(BaseConfig))
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
        result.IsSuccessful.Should().BeTrue();
    }
}
