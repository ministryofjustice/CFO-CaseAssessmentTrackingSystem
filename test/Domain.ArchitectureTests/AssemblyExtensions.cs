using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cfo.Cats.Domain.ArchitectureTests;

public static class AssemblyExtensions
{
    public static List<Type> GetConcreteTypesThatImplement(this Assembly assembly, Type interfaceType)
    {
        if (interfaceType.IsInterface == false)
        {
            throw new ArgumentException("The provided type must be an interface", nameof(interfaceType));
        }

        return assembly.GetTypes()
            .Where(type => type.IsAbstract == false && type.IsInterface == false && ImplementsInterface(type, interfaceType))
            .ToList();
    }

    private static bool ImplementsInterface(Type type, Type interfaceType)
    {
        if (interfaceType.IsAssignableFrom(type))
        {
            return true;
        }

        var currentType = type.BaseType;
        while (currentType != null && currentType != typeof(object))
        {
            if (interfaceType.IsAssignableFrom(currentType))
            {
                return true;
            }
            currentType = currentType.BaseType;
        }

        return false;
    }
}