using Mono.Cecil;
using NetArchTest.Rules;
using ONIONARCH.Application.Abstractions;

namespace ONIONARCH.Tests.ArchitectureTests.CustomRules
{
    internal class ICommandDbContextMustBeConstructorParameter : ICustomRule
    {
        public bool MeetsRule(TypeDefinition type)
        {
            bool isValid = true;
            foreach (var method in type.Methods.Where(x => x.IsConstructor))
            {
                isValid &= method.Parameters.Any(x => x.ParameterType.Name == typeof(ICommandDbContext).Name);
            }
            return isValid;
        }
    }
}
