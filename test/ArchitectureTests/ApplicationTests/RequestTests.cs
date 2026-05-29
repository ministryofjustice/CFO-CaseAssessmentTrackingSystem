using System.Linq;
using System.Reflection;
using Cfo.Cats.Application.Common.Security;
using Cortex.Mediator.Commands;
using Cortex.Mediator.Queries;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Domain.ArchitectureTests.ApplicationTests;

public class RequestTests
{
    private static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;

    [Test]
    public void Commands_Should_HaveAuthorizeAttribute()
    {
        var failedTypes = ApplicationAssembly.GetTypes()
            .Where(t => t.IsInterface == false)
            .Where(t => t.IsAbstract == false)
            .Where(t => t.ContainsGenericParameters == false)
            .Where(t => ImplementsOpenGenericInterface(t, typeof(ICommand<>))
                        || ImplementsOpenGenericInterface(t, typeof(IQuery<>)))
            .Where(t => t.GetCustomAttribute<RequestAuthorizeAttribute>() is null
                        && t.GetCustomAttribute<AllowAnonymousAttribute>() is null)
            .Select(t => t.FullName)
            .ToList();

        failedTypes
            .ShouldBeEmpty($"The following types failed the test:\n {string.Join("\n", failedTypes)}");
    }

    private static bool ImplementsOpenGenericInterface(Type type, Type openGenericInterface)
        => type.GetInterfaces().Any(i =>
            i.IsGenericType
            && i.GetGenericTypeDefinition() == openGenericInterface);
}