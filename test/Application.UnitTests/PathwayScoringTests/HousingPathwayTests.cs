﻿using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;
using DocumentFormat.OpenXml.InkML;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

namespace Cfo.Cats.Application.UnitTests.PathwayScoringTests;

public class HousingPathwayTests
{

    [Test]
    public void HousingPathway_Green()
    {
        HousingPathway pathway = new HousingPathway();

        pathway.B1.Answer = B1.HousingRentedOrOwnedByYouOrYourPartnerParentOrGuardian;
        pathway.B2.Answers = new List<string>() { B2.BehindOnRentOrMortgage };

        pathway.B4.Answer = B4.VerySatisfied;
        pathway.B5.Answer = B5.VerySafeToWalkAloneAfterDarkInLocalArea;

        var rag = pathway.GetRagScore(45, AssessmentLocation.EastMidlandsAssessmentLocation, Sex.FemaleSex);
        rag.Should().Be(77);
    }

    [Test]
    public void HousingPathway_Amber()
    {
        HousingPathway pathway = new HousingPathway();

        pathway.B1.Answer = B1.HousingRentedOrOwnedByYouOrYourPartnerParentOrGuardian;

        pathway.B2.Answers = new List<string>() { B2.BehindOnRentOrMortgage, B2.FacingEviction, B2.RiskOfHomelessness }; ;

        pathway.B4.Answer = B4.VerySatisfied;
        pathway.B5.Answer = B5.VerySafeToWalkAloneAfterDarkInLocalArea;

        var rag = pathway.GetRagScore(45, AssessmentLocation.EastMidlandsAssessmentLocation, Sex.FemaleSex);
        rag.Should().Be(13);
    }

    [Test]
    public void HousingPathway_Red()
    {
        HousingPathway pathway = new HousingPathway();

        pathway.B1.Answer = B1.TemporaryOrSupportedHousing;

        pathway.B2.Answers = new List<string>() { B2.BehindOnRentOrMortgage, B2.RiskOfHomelessness };

        pathway.B4.Answer = B4.VeryDissatisfied;
        pathway.B5.Answer = B5.ABitUnsafeToWalkAloneAfterDarkInLocalArea;

        var rag = pathway.GetRagScore(45, AssessmentLocation.EastMidlandsAssessmentLocation, Sex.FemaleSex);
        rag.Should().Be(0);
    }
}
