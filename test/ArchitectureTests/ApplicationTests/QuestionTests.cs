using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Cfo.Cats.Application.Features.Assessments.DTOs;
using FluentAssertions;
using FluentAssertions.Execution;
using NetArchTest.Rules;
using NUnit.Framework;

namespace Cfo.Cats.Domain.ArchitectureTests.ApplicationTests;

public class QuestionTests
{
    private static readonly Assembly ApplicationAssembly = typeof(Application.DependencyInjection).Assembly;

    [Test]
    public void All_Derived_Classes_Should_Be_Listed_In_JsonDerivedType_Attributes()
    {
        // Get the SingleChoiceQuestion type
        var questionType = typeof(QuestionBase);

        // Get all JsonDerivedType attributes on the SingleChoiceQuestion class
        var attributes = questionType.GetCustomAttributes<JsonDerivedTypeAttribute>().ToList();

        // Get the list of types specified in the JsonDerivedType attributes
        var listedTypes = attributes.Select(attr => attr.DerivedType).ToHashSet();

        // Get all types inheriting from SingleChoiceQuestion
        var derivedTypes = Types.InAssembly(ApplicationAssembly)
            .That()
            .Inherit(questionType)
            .And()
            .AreNotAbstract() 
            .GetTypes()
            .ToList();

        // Collect failing types
        var failingTypes = new List<string>();

        foreach (var derivedType in derivedTypes)
        {
            if (!listedTypes.Contains(derivedType))
            {
                failingTypes.Add(derivedType.Name);
            }
        }

        // Assert that there are no failing types
        failingTypes.Should().BeEmpty($"The following derived types are not listed in the JsonDerivedType attributes on QuestionBase: {string.Join(", ", failingTypes)}");
        
    }
    
   
    
}
