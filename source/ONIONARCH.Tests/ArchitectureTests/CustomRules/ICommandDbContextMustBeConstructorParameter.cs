using Mono.Cecil;
using NetArchTest.Rules;
using ONIONARCH.Application.Abstractions.Context;
using ONIONARCH.Application.Abstractions.Repositories;

namespace ONIONARCH.Tests.ArchitectureTests.CustomRules;

/// <summary>
/// Requires a command handler's constructor to take a real command-side abstraction —
/// <see cref="ICommandDbContext"/> for the EF Core path, or <see cref="ISampleEntityDapperCommandRepository"/>
/// for the Dapper path. Deliberately does NOT accept a raw connection factory
/// (e.g. IDbWriteConnectionFactory): allowing that would let a handler open an
/// IDbConnection and run ad-hoc SQL directly in Application, which is the violation
/// this rule exists to prevent.
/// </summary>
internal class ICommandDbContextMustBeConstructorParameter : ICustomRule
{
    /// <summary>
    /// Checks that every constructor of <paramref name="type"/> has a parameter whose type name is
    /// <see cref="ICommandDbContext"/> or <see cref="ISampleEntityDapperCommandRepository"/>.
    /// </summary>
    /// <param name="type">The Mono.Cecil definition of the type under test.</param>
    /// <returns><see langword="true"/> if every constructor satisfies the rule; otherwise <see langword="false"/>.</returns>
    /// <remarks>Parameter types are compared by simple name only, not by namespace.</remarks>
    public bool MeetsRule(TypeDefinition type)
    {
        bool isValid = true;
        foreach (var method in type.Methods.Where(x => x.IsConstructor))
        {
            isValid &= method.Parameters.Any(x => x.ParameterType.Name == typeof(ICommandDbContext).Name)
                || method.Parameters.Any(x => x.ParameterType.Name == typeof(ISampleEntityDapperCommandRepository).Name);
        }
        return isValid;
    }
}
