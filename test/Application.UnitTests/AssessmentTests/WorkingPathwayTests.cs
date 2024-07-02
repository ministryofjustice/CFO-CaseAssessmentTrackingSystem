using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;
using FluentAssertions;
using NUnit.Framework;

namespace Cfo.Cats.Application.UnitTests.AssessmentTests;

public class WorkingPathwayTests
{
    [Test]
    [TestCase(18, "Unknown", "Male", "Looking for work", 0.5D)]
    [TestCase(35, "Unknown", "Female", "Looking for work", 0.5D)]
    [TestCase(35, "Unknown", "Male", "Do not want a job", 1.0D)]
    public void A1_Question_ShouldCalculateProperly(int age, string location, string sex, string answer, double expected)
    {
        var a1 = new A1
        {
            Answer = answer
        };

        a1.Score(age, AssessmentLocation.FromName(location), Sex.FromName(sex))
            .Should()
            .Be(expected);

    }
    
    [Test]
    [TestCase(18, "Unknown", "Male", "Over a year ago", 0.79683D)]
    [TestCase(35, "Unknown", "Female", "Over a year ago", 0.79683D)]
    [TestCase(35, "Unknown", "Male", "Do not want a job", 1.0D)]
    public void A2_Question_ShouldCalculateProperly(int age, string location, string sex, string answer, double expected)
    {
        var a2 = new A2()
        {
            Answer = answer
        };

        a2.Score(age, AssessmentLocation.FromName(location), Sex.FromName(sex))
            .Should()
            .Be(expected);

    }
    
    
    [Test]
    public void WorkingPathway_Should_Calculate_Properly()
    {   
        WorkingPathway pathway = new WorkingPathway();
        pathway.A1.Answer = "Looking for work";
        pathway.A2.Answer = "Over a year ago";
        
            pathway.Score(18, AssessmentLocation.Unknown, Sex.Male)
            .Should()
            .Be(0.16215D);
        
    }
    
    
}

