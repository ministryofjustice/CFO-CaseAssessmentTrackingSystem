using System.Linq;
using System.Reflection;
using Cfo.Cats.Application.Common.Interfaces.Caching;
using Cfo.Cats.Application.Common.Security;
using MediatR;
using NetArchTest.Rules;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Domain.ArchitectureTests.ApplicationTests;

public class RequestTests
{
    private static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;

    [Test]
    public void Commands_Should_HaveAuthorizeAttribute()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .That()
            .AreNotInterfaces()
            .And()
            .AreNotAbstract()
            .And()
            .ImplementInterface(typeof(IRequest<>))
            .Or()
            .ImplementInterface(typeof(ICacheableRequest<>))
            .Or()
            .ImplementInterface(typeof(ICacheInvalidatorRequest<>))
            .Should()
            .HaveCustomAttribute(typeof(RequestAuthorizeAttribute))
            .Or()
            .HaveCustomAttribute(typeof(AllowAnonymousAttribute))
            .GetResult();

        var failedTypes = result.FailingTypes?.Select(t => t.FullName).ToList();
        
        var formattedFailedTypes = failedTypes == null ? "None" : string.Join("\n", failedTypes);
        
        result.IsSuccessful
            .ShouldBeTrue($"The following types failed the test:\n {formattedFailedTypes}");
    }
}