using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
using DocumentFormat.OpenXml.InkML;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Cfo.Cats.Application.UnitTests.PathwayScoringTests;

public class EducationPathwayTests
{

    [Test]
    public void EducationPathway_Green()
    {
        EducationPathway pathway = new EducationPathway();

        pathway.D1.Answer = D1.FivePlusGcsesOrSimilar;

        pathway.D2.Answer = D2.YesFinishedSchool;

        pathway.D3.Answers = new List<string>() { D3.NoneOftheGivenOptions }; ;

        pathway.D4.Answers = new List<string>() { D4.UsingNumbersDifficulty }; ;


        pathway.D5.Answers = new List<string>() { D5.Reading, D5.Drawing, D5.Painting, D5.Photography };

        var rag = pathway.GetRagScore(23, AssessmentLocation.EastOfEnglandAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(25);
    }

    [Test]
    public void MoneyPathway_Amber()
    {
        EducationPathway pathway = new EducationPathway();

        pathway.D1.Answer = D1.NvqLevelOneOrSimilar;

        pathway.D2.Answer = D2.YesFinishedSchool;

        pathway.D3.Answers = new List<string>() { D3.NoneOftheGivenOptions };

        pathway.D4.Answers = new List<string>() { D4.ReadingDifficulty, D4.WritingDifficulty };

        pathway.D5.Answers = new List<string>() { D5.Crafts };

        var rag = pathway.GetRagScore(23, AssessmentLocation.EastOfEnglandAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(11);
    }

    [Test]
    public void MoneyPathway_Red()
    {
        EducationPathway pathway = new EducationPathway();

        pathway.D1.Answer = D1.NoQualificationsOrEntryLevel;

        pathway.D2.Answer = D2.LeftBefore16;

        pathway.D3.Answers = new List<string>() { D3.Dyslexia, D3.AdhdOrAdd };

        pathway.D4.Answers = new List<string>() { D4.ReadingDifficulty, D4.WritingDifficulty };

        pathway.D5.Answers = new List<string>() { D5.Crafts };

        var rag = pathway.GetRagScore(23, AssessmentLocation.EastOfEnglandAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(1);
    }
}
