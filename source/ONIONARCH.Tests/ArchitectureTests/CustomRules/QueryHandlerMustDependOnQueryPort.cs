using Mono.Cecil;
using NetArchTest.Rules;
//#if (HasDapper)
using ONIONARCH.Application.Abstractions;
//#endif
//#if (HasEfCore)
using ONIONARCH.Application.Abstractions.Context;
//#endif

namespace ONIONARCH.Tests.ArchitectureTests.CustomRules;

/// <summary>
/// Requires every constructor of a query handler to take a query-side persistence port. Depending on
/// the persistence paths the solution includes, that is the EF Core <c>IQueryDbContext</c> and/or a Dapper
/// repository port: any type in the Application layer's <c>Abstractions.Repositories</c> namespace whose
/// name ends in <c>QueryRepository</c>. A command-side port does not qualify, which keeps the CQRS sides
/// apart. Deliberately does NOT accept a raw connection factory (e.g. <c>IDbReadOnlyConnectionFactory</c>): allowing that
/// would let a handler open an <c>IDbConnection</c> and run ad-hoc SQL directly in Application, which is
/// the violation this rule exists to prevent.
/// </summary>
internal sealed class QueryHandlerMustDependOnQueryPort : ICustomRule
{
//#if (HasDapper)
    /// <summary>
    /// The namespace holding the Dapper repository ports, derived from a type that always exists so it
    /// follows the solution's name.
    /// </summary>
    private static readonly string RepositoryPortsNamespace = $"{typeof(ISender).Namespace}.Repositories";

//#endif
    /// <summary>
    /// Checks that every constructor of <paramref name="type"/> has at least one query-side port parameter.
    /// </summary>
    /// <param name="type">The Mono.Cecil definition of the type under test.</param>
    /// <returns><see langword="true"/> if every constructor satisfies the rule; otherwise <see langword="false"/>.</returns>
    public bool MeetsRule(TypeDefinition type)
    {
        bool isValid = true;
        foreach (var method in type.Methods.Where(x => x.IsConstructor))
        {
            isValid &= method.Parameters.Any(x => IsQueryPort(x.ParameterType));
        }
        return isValid;
    }

    /// <summary>
    /// Determines whether <paramref name="parameterType"/> is a query-side persistence port.
    /// </summary>
    /// <param name="parameterType">The constructor parameter's type.</param>
    /// <returns><see langword="true"/> for a query-side port; otherwise <see langword="false"/>.</returns>
    private static bool IsQueryPort(TypeReference parameterType)
    {
//#if (HasEfCore)
        if (parameterType.FullName == typeof(IQueryDbContext).FullName)
        {
            return true;
        }

//#endif
//#if (HasDapper)
        if (parameterType.Namespace == RepositoryPortsNamespace
            && parameterType.Name.EndsWith("QueryRepository", StringComparison.Ordinal))
        {
            return true;
        }

//#endif
        return false;
    }
}
