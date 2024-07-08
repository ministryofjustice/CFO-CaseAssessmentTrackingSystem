using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Relationships;
using DocumentFormat.OpenXml.InkML;
using FluentAssertions;
using NUnit.Framework;

namespace Cfo.Cats.Application.UnitTests.PathwayScoringTests;

public class RelationshipsnPathwayTests
{

    [Test]
    public void RelationshipsnPathway_Green()
    {
        RelationshipsPathway pathway = new RelationshipsPathway();

        pathway.F1.Answer = F1.ExtremelyHappy;

        pathway.F2.Answer = F2.HardlyEver;

        pathway.F5.Answer = F5.AtleastOncePerWeek;

        pathway.F6.Answer = F6.Somewhat;

        pathway.F8.Answer = F8.No;

        var rag = pathway.GetRagScore(60, AssessmentLocation.SouthEastAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(36);
    }

    [Test]
    public void HealthAndAddictionPathway_Amber()
    {
        RelationshipsPathway pathway = new RelationshipsPathway();

        pathway.F1.Answer = F1.ExtremelyHappy;

        pathway.F2.Answer = F2.Occasionally;

        pathway.F5.Answer = F5.LessThanOncePerWeek;

        pathway.F6.Answer = F6.Somewhat;

        pathway.F8.Answer = F8.No;

        var rag = pathway.GetRagScore(60, AssessmentLocation.SouthEastAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(24);
    }

    [Test]
    public void HealthAndAddictionPathway_Red()
    {
        RelationshipsPathway pathway = new RelationshipsPathway();

        pathway.F1.Answer = F1.ExtremelyHappy;

        pathway.F2.Answer = F2.Occasionally;

        pathway.F5.Answer = F5.LessThanOncePerWeek;

        pathway.F6.Answer = F6.NotAtAll;

        pathway.F8.Answer = F8.No;

        var rag = pathway.GetRagScore(60, AssessmentLocation.SouthEastAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(3);
    }

}
