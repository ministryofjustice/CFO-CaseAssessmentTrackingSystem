using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.HealthAndAdditiction;
using DocumentFormat.OpenXml.InkML;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Cfo.Cats.Application.UnitTests.PathwayScoringTests;

public class HealthAndAddictionPathwayTests
{

    [Test]
    public void HealthAndAddictionPathway_Green()
    {
        HealthAndAddictionPathway pathway = new HealthAndAddictionPathway();

        pathway.E1.Answer = E1.Good;

        pathway.E2.Answer = E2.No;

        pathway.E4.Answer = E4.RegisteredWithGPOnly;

        pathway.E5.Answer = E5.Over2AndHalfHoursPerWeek;

        pathway.E6.Answers = [ E6.SexOrPornography, E6.PrescriptionDrugs, E6.Food, E6.SomethingElse ];

        pathway.E8.Answer = E8.NeverSmoked;
        pathway.E9.Answer = E9.NeverVaped;

        var rag = pathway.GetRagScore(41, AssessmentLocation.NorthWestAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(29);
    }

    [Test]
    public void HealthAndAddictionPathway_Amber()
    {
        HealthAndAddictionPathway pathway = new HealthAndAddictionPathway();

        pathway.E1.Answer = E1.Good;

        pathway.E2.Answer = E2.LittleImpairment;

        pathway.E4.Answer = E4.RegisteredWithGPOnly;

        pathway.E5.Answer = E5.Over2AndHalfHoursPerWeek;

        pathway.E6.Answers = [ E6.Alcohol, E6.SexOrPornography, E6.PrescriptionDrugs, E6.Food, E6.SomethingElse ];

        pathway.E8.Answer = E8.IUsedTo;
        pathway.E9.Answer = E9.NeverVaped;

        var rag = pathway.GetRagScore(41, AssessmentLocation.NorthWestAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(13);
    }

    [Test]
    public void HealthAndAddictionPathway_Red()
    {
        HealthAndAddictionPathway pathway = new HealthAndAddictionPathway();

        pathway.E1.Answer = E1.Good;

        pathway.E2.Answer = E2.LittleImpairment;

        pathway.E4.Answer = E4.RegisteredWithGPOnly;

        pathway.E5.Answer = E5.Over2AndHalfHoursPerWeek;

        pathway.E6.Answers = [ E6.Alcohol, E6.IllegalDrugsOrSubstances, E6.SexOrPornography, E6.PrescriptionDrugs, E6.Food, E6.SomethingElse ];

        pathway.E8.Answer = E8.Yes;
        pathway.E9.Answer = E9.NeverVaped;

        var rag = pathway.GetRagScore(41, AssessmentLocation.NorthWestAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(4);
    }
}
