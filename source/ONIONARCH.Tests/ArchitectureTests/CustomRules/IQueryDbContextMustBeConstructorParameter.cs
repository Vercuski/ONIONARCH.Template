using ONIONARCH.Application.Abstractions;
using Mono.Cecil;
using NetArchTest.Rules;
using NUnit.Framework.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ONIONARCH.Tests.ArchitectureTests.CustomRules
{
    internal class IQueryDbContextMustBeConstructorParameter : ICustomRule
    {
        public bool MeetsRule(TypeDefinition type)
        {
            bool isValid = true;
            foreach(var method in type.Methods.Where(x => x.IsConstructor))
            {
                isValid &= method.Parameters.Any(x => x.ParameterType.Name == typeof(IQueryDbContext).Name);
            }
            return isValid;
        }
    }
}
