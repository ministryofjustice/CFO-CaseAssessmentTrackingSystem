using System.Linq;
using System.Reflection;
using Cfo.Cats.Domain.Common;
using Cfo.Cats.Domain.Common.Contracts;
using FluentAssertions;
using NetArchTest.Rules;
using NUnit.Framework;

namespace Cfo.Cats.Domain.ArchitectureTests;

public class EntityTests
{
    private static readonly Assembly DomainAssembly = typeof(IEntity).Assembly;
    
    [Test]
    public void DomainEvents_Should_BeSealed()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(DomainEvent))
            .And()
            .AreNotAbstract()
            .Should()
            .BeSealed()
            .GetResult();
        

        var failedTypes = result.FailingTypes?.Select(t => t.FullName).ToList();
        
        var formattedFailedTypes = failedTypes == null ? "None" : string.Join("\n", failedTypes);

        
        result.IsSuccessful
            .Should()
            .BeTrue($"The following types failed the test:\n {formattedFailedTypes}");
    
    }
}
