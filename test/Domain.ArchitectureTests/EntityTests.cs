using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Entities;
using FluentAssertions;
using NetArchTest.Rules;
using NUnit.Framework;

namespace Cfo.Cats.Domain.ArchitectureTests;

public class EntityTests
{
    private static readonly Assembly DomainAssembly = typeof(IEntity).Assembly;
    
    [Test]
    public void Entities_Should_HavePrivateParameterlessConstructor()
    {
        var entityTypes = DomainAssembly.GetConcreteTypesThatImplement(typeof(IEntity));

        var failingTypes = new List<Type>();

        foreach (var type in entityTypes)
        {
            if (type == typeof(AuditTrail))
            {
                continue;
            }
            
            var constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

            if (constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0) == false)
            {
                failingTypes.Add(type);
            }
        }

        failingTypes.Should().BeEmpty();

    }
}