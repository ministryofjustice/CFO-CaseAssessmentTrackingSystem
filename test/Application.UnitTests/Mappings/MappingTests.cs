using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using AutoMapper;
using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Tenants.DTOs;
using Cfo.Cats.Domain.Entities.Administration;
using Cfo.Cats.Domain.Entities.Participants;
using NUnit.Framework;

namespace Cfo.Cats.Application.UnitTests.Mappings;

public class MappingTests
{
    private readonly IConfigurationProvider configuration;
    private readonly IMapper mapper;

    public MappingTests()
    {
        configuration = new MapperConfiguration(cfg => cfg.AddMaps(Assembly.GetAssembly(typeof(IApplicationDbContext))));
        mapper = configuration.CreateMapper();
    }
    
    [Test]
    public void ShouldHaveValidConfiguration()
    {
        configuration.AssertConfigurationIsValid();
    }

    [Test]
    [TestCase(typeof(Participant), typeof(ParticipantDto), false)]
    [TestCase(typeof(Tenant), typeof(TenantDto), false)]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination, bool inverseMap = false)
    {
        var instance = GetInstanceOf(source);

        mapper.Map(instance, source, destination);

        if (inverseMap)
        {
            ShouldSupportMappingFromSourceToDestination(destination, source);
        }
    }

    [Test]
    public void LocationDtoShouldMapCorrectlyToLocationTypeDto()
    {
           
        
    }

    private static object GetInstanceOf(Type type)
    {
        // Check for a parameterless constructor, including non-public ones
        ConstructorInfo constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
        if (constructor != null)
            return constructor.Invoke(null);

        // Type without parameterless constructor
        return  RuntimeHelpers.GetUninitializedObject(type);
    }
    
}