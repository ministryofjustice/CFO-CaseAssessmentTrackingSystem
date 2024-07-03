using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;
using DocumentFormat.OpenXml.InkML;
using FluentAssertions;
using NUnit.Framework;

namespace Cfo.Cats.Application.UnitTests.PathwayScoringTests;

public class WorkingPathwayTests
{
    [Test]
    public void Working_Pathway_Percentiles_Are_Correct()
    {

        WorkingPathway pathway = new WorkingPathway();

        pathway.A1.Answer = A1.LookingForWork;
        pathway.A2.Answer = A2.OverAYearAgo;
        // we do not score A3
        pathway.A3.Answer = A3.No;
        
        pathway.A4.Answer = A4.NoPreferNot;
        pathway.A5.Answer = A5.Yes;
        pathway.A6.Answer = A6.No;
        pathway.A7.Answer = A7.No;
        pathway.A8.Answer = A8.Yes;
        pathway.A9.Answer = A9.No;
        
        
        var cumulativePercentile = pathway.GetPercentile(18, AssessmentLocation.EastMidlandsAssessmentLocation, Sex.MaleSex);

        cumulativePercentile.Should().Be(0.16215);

    }

    [Test]
    public void WorkingPathway_RagScore_Is_Correct()
    {
        WorkingPathway pathway = new WorkingPathway();

        pathway.A1.Answer = A1.LookingForWork;
        pathway.A2.Answer = A2.OverAYearAgo;
        // we do not score A3
        pathway.A3.Answer = A3.No;
        
        pathway.A4.Answer = A4.NoPreferNot;
        pathway.A5.Answer = A5.Yes;
        pathway.A6.Answer = A6.No;
        pathway.A7.Answer = A7.No;
        pathway.A8.Answer = A8.Yes;
        pathway.A9.Answer = A9.No;

        var rag = pathway.GetRagScore(18, AssessmentLocation.EastMidlandsAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(2);
    }
}
