using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cfo.Cats.Application.Common.Interfaces;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
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

        List<string> warnings = new List<string>();
        List<string> failures = new List<string>();


        foreach (var handlerType in handlerTypes)
        {
            // Get constructors
            var constructors = handlerType.GetConstructors();
            foreach (var constructor in constructors)
            {
                // Get parameters of the constructor
                var parameters = constructor.GetParameters();

                if (parameters.Length == 0)
                {
                    warnings.Add($"Type {handlerType.FullName} has a parameterless constructor. Check this is correct.");
                    break;
                }

                // Ensure none of the parameters is of type IApplicationDbContext
                if(parameters.Any(p => p.ParameterType == typeof(IApplicationDbContext)))
                {
                    failures.Add($"{handlerType.FullName} should not accept IApplicationDbContext as a constructor parameter.");
                }


                // Ensure there is a parameter of type IUnitOfWork
                if (parameters.Any(p => p.ParameterType == typeof(IUnitOfWork)) == false)
                {
                    failures.Add($"{handlerType.FullName} should accept IUnitOfWork as a constructor parameter.");
                }
            }
        }

        failures.Should().BeEmpty(string.Join("\n", failures));

        if (warnings.Any())
        {
            Assert.Warn(string.Join("\n", warnings));
        }
    }
}
