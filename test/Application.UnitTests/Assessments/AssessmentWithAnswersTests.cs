#nullable enable
using Cfo.Cats.Application.Features.Assessments.DTOs;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Education;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Housing;
using Cfo.Cats.Application.Features.Assessments.DTOs.V1.Pathways.Working;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Assessments;

public class AssessmentWithAnswersTests
{
    private Assessment _assessment = null!;

    [SetUp]
    public void Setup() => _assessment = new Assessment
    {
        Id = Guid.NewGuid(),
        ParticipantId = "A12345678",
        Pathways =
        [
            new WorkingPathway(),
            new HousingPathway(),
            new EducationPathway(),
        ]
    };

    [Test]
    public void WithAnswers_PopulatesSingleChoiceAnswer()
    {
        var lookup = new[]
        {
            ("B1", "Sleep rough"),
            ("A1", "Looking for work"),
        }.ToLookup(x => x.Item1, x => x.Item2);

        _assessment.WithAnswers(lookup);

        var housing = (HousingPathway)_assessment.Pathways[1];
        housing.B1.Answer.ShouldBe("Sleep rough");

        var working = (WorkingPathway)_assessment.Pathways[0];
        working.A1.Answer.ShouldBe("Looking for work");
    }

    [Test]
    public void WithAnswers_PopulatesMultipleChoiceAnswers()
    {
        var lookup = new[]
        {
            ("D3", "Dyslexia"),
            ("D3", "ADHD / ADD"),
        }.ToLookup(x => x.Item1, x => x.Item2);

        _assessment.WithAnswers(lookup);

        var education = (EducationPathway)_assessment.Pathways[2];
        education.D3.Answers.ShouldNotBeNull();
        education.D3.Answers!.ShouldContain("Dyslexia");
        education.D3.Answers!.ShouldContain("ADHD / ADD");
    }

    [Test]
    public void WithAnswers_LeavesUnansweredQuestionsNull()
    {
        var lookup = Enumerable.Empty<(string, string)>()
            .ToLookup(x => x.Item1, x => x.Item2);

        _assessment.WithAnswers(lookup);

        var housing = (HousingPathway)_assessment.Pathways[1];
        housing.B1.Answer.ShouldBeNull();
    }

    [Test]
    public void WithAnswers_ReturnsTheSameAssessmentInstance()
    {
        var lookup = Enumerable.Empty<(string, string)>()
            .ToLookup(x => x.Item1, x => x.Item2);

        var result = _assessment.WithAnswers(lookup);

        result.ShouldBeSameAs(_assessment);
    }

    [Test]
    public void QuestionCode_MatchesClassName()
    {
        var working = (WorkingPathway)_assessment.Pathways[0];
        working.A1.Code.ShouldBe("A1");
        working.A10.Code.ShouldBe("A10");

        var education = (EducationPathway)_assessment.Pathways[2];
        education.D3.Code.ShouldBe("D3");
    }
}
