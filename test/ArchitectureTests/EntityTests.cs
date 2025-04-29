using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cfo.Cats.Domain.Common.Contracts;
using NUnit.Framework;
using Shouldly;

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
            var constructors = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);

            if (constructors.Any(c => c.IsPrivate && c.GetParameters().Length == 0) == false)
            {
                failingTypes.Add(type);
            }
        }

        failingTypes.ShouldBeEmpty();
    }
}