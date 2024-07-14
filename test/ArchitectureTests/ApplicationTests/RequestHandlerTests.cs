using System.Linq;
using System.Reflection;
using Cfo.Cats.Application.Common.Interfaces;
using FluentAssertions;
using MediatR;
using NUnit.Framework;

namespace Cfo.Cats.Domain.ArchitectureTests.ApplicationTests;

public class RequestHandlerTests
{
    private static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;
    
    [Test]
    public void HandlersDoNotReferToDbContextDirectly()
    {
        // Get all types implementing IRequestHandler
        var handlerTypes = ApplicationAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();
        
        foreach (var handlerType in handlerTypes)
        {
            // Get constructors
            var constructors = handlerType.GetConstructors();
            foreach (var constructor in constructors)
            {
                // Get parameters of the constructor
                var parameters = constructor.GetParameters();

                // Ensure none of the parameters is of type IApplicationDbContext
                parameters.Should().NotContain(p => p.ParameterType == typeof(IApplicationDbContext),
                $"{handlerType.Name} should not accept IApplicationDbContext as a constructor parameter.");
                
                // Ensure there is a parameter of type IUnitOfWork
                parameters.Should().Contain(p => p.ParameterType == typeof(IUnitOfWork),
                $"{handlerType.Name} should have IUnitOfWork as a constructor parameter.");
            }
        }
    }
}
