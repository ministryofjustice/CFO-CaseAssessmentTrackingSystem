using System.Linq;
using System.Reflection;
using Cfo.Cats.Domain.Common;
using Cfo.Cats.Domain.Common.Contracts;
using NetArchTest.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Domain.ArchitectureTests;

public class DomainEventTests
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
            .ShouldBeTrue($"The following types failed the test:\n {formattedFailedTypes}");    
    }

    [Test]
    public void DomainEvents_Should_HaveDomainEventPostfix()
    {
        var result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(DomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .Or()
            .HaveNameEndingWith("DomainEvent`1") // this allows for our generic type
            .GetResult();
        
        var failedTypes = result.FailingTypes?.Select(t => t.FullName).ToList();
        
        var formattedFailedTypes = failedTypes == null ? "None" : string.Join("\n", failedTypes);
        
        result.IsSuccessful
            .ShouldBeTrue($"The following types failed the test:\n {formattedFailedTypes}");
    }
}