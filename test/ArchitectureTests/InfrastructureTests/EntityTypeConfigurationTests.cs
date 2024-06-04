using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cfo.Cats.Application.Pipeline;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Cfo.Cats.Domain.ArchitectureTests.InfrastructureTests;

public class EntityConfigurationTests
{
    private static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.DependencyInjection).Assembly;

    [Test]
    public void EntityConfigurations_Should_HaveConfigurationPostfix()
    {
        var types = InfrastructureAssembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .Where(type => type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
            .ToList();

        var failingTypes = new List<Type>();

        foreach (var type in types)
        {
            if (type.Name.EndsWith("Configuration") == false)
            {
                failingTypes.Add(type);
            }
        }

        failingTypes.Should().BeEmpty();

    }

}