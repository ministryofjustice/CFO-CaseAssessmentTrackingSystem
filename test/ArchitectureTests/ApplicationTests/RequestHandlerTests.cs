using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Infrastructure.Persistence;
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
        var handlerTypes = ApplicationAssembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>)))
            .ToList();

        List<string> failures = [];

        foreach (var handlerType in handlerTypes)
        {
            var constructors = handlerType.GetConstructors();
            foreach (var constructor in constructors)
            {
                var parameters = constructor.GetParameters();

                if(parameters.Any(p => p.ParameterType == typeof(IApplicationDbContext)))
                {
                    failures.Add($"{handlerType.FullName} should not accept IApplicationDbContext as a constructor parameter.");
                }

                if (parameters.Any(p => p.ParameterType == typeof(ApplicationDbContext)))
                {
                    failures.Add($"{handlerType.FullName} should not accept ApplicationDbContext as a constructor parameter.");
                }
            }
        }

        failures.Should()
            .BeEmpty(string.Join(Environment.NewLine, failures));
    }
}
