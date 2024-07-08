using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Money;
using DocumentFormat.OpenXml.InkML;
using FluentAssertions;
using NUnit.Framework;

namespace Cfo.Cats.Application.UnitTests.PathwayScoringTests;

public class MoneyPathwayTests
{

    [Test]
    public void MoneyPathway_Green()
    {
        MoneyPathway pathway = new MoneyPathway();

        //Coping Financially
        pathway.C1.Answer = C1.DoingAlright;

        //Paying Bills
        pathway.C2.Answer = C2.No;

        //Problem Debt
        //Do you owe over £1000 in unsecured loans, credit cards or other debt
        pathway.C3.Answer = C3.No;

        //Bank Account
        pathway.C5.Answer = C5.Yes;

        //Food Security - Used a food bank
        pathway.C9.Answer = C9.Never;

        var rag = pathway.GetRagScore(32, AssessmentLocation.SouthWestAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(23);
    }

    [Test]
    public void MoneyPathway_Amber()
    {
        MoneyPathway pathway = new MoneyPathway();

        //Coping Financially
        pathway.C1.Answer = C1.DoingAlright;

        //Paying Bills
        pathway.C2.Answer = C2.No;

        //Problem Debt
        //Do you owe over £1000 in unsecured loans, credit cards or other debt
        pathway.C3.Answer = C3.Yes;

        //Bank Account
        pathway.C5.Answer = C5.Yes;

        //Food Security - Used a food bank
        pathway.C9.Answer = C9.WithinTheLastYear;

        var rag = pathway.GetRagScore(32, AssessmentLocation.SouthWestAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(14);
    }

    [Test]
    public void MoneyPathway_Red()
    {
        MoneyPathway pathway = new MoneyPathway();

        //Coping Financially
        pathway.C1.Answer = C1.JustAboutGettingBy;

        //Paying Bills
        pathway.C2.Answer = C2.No;

        //Problem Debt
        //Do you owe over £1000 in unsecured loans, credit cards or other debt
        pathway.C3.Answer = C3.Yes;
        //Do you owe any informal debt
        pathway.C4.Answer = C4.Yes;

        //Bank Account
        pathway.C5.Answer = C5.Yes;

        //Food Security - Used a food bank
        pathway.C9.Answer = C9.WithinTheLastYear;

        var rag = pathway.GetRagScore(32, AssessmentLocation.SouthWestAssessmentLocation, Sex.MaleSex);
        rag.Should().Be(6);
    }
}
