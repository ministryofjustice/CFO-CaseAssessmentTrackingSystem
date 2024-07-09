using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.WellbeingAndMentalHealth;
using DocumentFormat.OpenXml.InkML;
using FluentAssertions;
using NUnit.Framework;

namespace Cfo.Cats.Application.UnitTests.PathwayScoringTests;

public class WellbeingAndMentalHealthPathwayTests
{

    [Test]
    public void WellbeingAndMentalHealthPathway_Green()
    {
        WellbeingAndMentalHealthPathway pathway = new WellbeingAndMentalHealthPathway();

        pathway.H1.Answer = H1.NotAtAll;

        pathway.H2.Answer = H2.Often;

        pathway.H3.Answer = H3.OnlyOccasionally;

        pathway.H4.Answer = H4.NotAtAll;

        pathway.H5.Answer = H5.NotAtAll;

        pathway.H6.Answer = H6.OnlyOccasionally;

        pathway.H7.Answer = H7.Sometimes;

        pathway.H8.Answer = H8.Sometimes;

        pathway.H9.Answer = H9.NotAtAll;

        pathway.H10.Answer = H10.MostOrAllOfTheTime;

        //Satisfied with your life
        pathway.H11.Answer = H11.Mostly;
        //Stress
        pathway.H12.Answer = H12.FewTimesAMonth;



        var rag = pathway.GetRagScore(36, AssessmentLocation.NorthWestAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(35);
    }

    [Test]
    public void WellbeingAndMentalHealthPathway_Amber()
    {
        WellbeingAndMentalHealthPathway pathway = new WellbeingAndMentalHealthPathway();

        pathway.H1.Answer = H1.NotAtAll;

        pathway.H2.Answer = H2.Often;

        pathway.H3.Answer = H3.OnlyOccasionally;

        pathway.H4.Answer = H4.NotAtAll;

        pathway.H5.Answer = H5.NotAtAll;

        pathway.H6.Answer = H6.Often;

        pathway.H7.Answer = H7.Sometimes;

        pathway.H8.Answer = H8.Often;

        pathway.H9.Answer = H9.NotAtAll;

        pathway.H10.Answer = H10.MostOrAllOfTheTime;

        //Satisfied with your life
        pathway.H11.Answer = H11.Mostly;
        //Stress
        pathway.H12.Answer = H12.FewTimesAMonth;



        var rag = pathway.GetRagScore(36, AssessmentLocation.NorthWestAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(17);
    }

    [Test]
    public void WellbeingAndMentalHealthPathway_Red()
    {
        WellbeingAndMentalHealthPathway pathway = new WellbeingAndMentalHealthPathway();

        pathway.H1.Answer = H1.NotAtAll;

        pathway.H2.Answer = H2.Often;

        pathway.H3.Answer = H3.OnlyOccasionally;

        pathway.H4.Answer = H4.NotAtAll;

        pathway.H5.Answer = H5.NotAtAll;

        pathway.H6.Answer = H6.Often;

        pathway.H7.Answer = H7.Sometimes;

        pathway.H8.Answer = H8.Often;

        pathway.H9.Answer = H9.NotAtAll;

        pathway.H10.Answer = H10.MostOrAllOfTheTime;

        //Satisfied with your life
        pathway.H11.Answer = H11.MostlyNot;
        //Stress
        pathway.H12.Answer = H12.FewTimesAMonth;



        var rag = pathway.GetRagScore(36, AssessmentLocation.NorthWestAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(1);
    }
}
