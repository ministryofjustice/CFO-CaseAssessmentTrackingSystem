#nullable enable
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.ValueObjects;
using NUnit.Framework;
using Shouldly;

namespace Cfo.Cats.Application.UnitTests.Assessments;

public class ParticipantAssessmentAnswerTests
{
    private ParticipantAssessment _assessment = null!;

    [SetUp]
    public void Setup() => _assessment = ParticipantAssessment.Create(
            Guid.NewGuid(),
            participantId: "A12345678",
            tenantId: "1.",
            locationId: 1
        );

    [Test]
    public void SetAnswer_StoresAnswerByQuestionCode()
    {
        _assessment.SetAnswer("B1", "Sleep rough");

        _assessment.GetAnswer("B1").ShouldBe("Sleep rough");
    }

    [Test]
    public void SetAnswer_ReplacesExistingAnswer()
    {
        _assessment.SetAnswer("B1", "Sleep rough");
        _assessment.SetAnswer("B1", "Housing is rented or owned by you, your partner, parent or guardian");

        _assessment.GetAnswer("B1").ShouldBe("Housing is rented or owned by you, your partner, parent or guardian");
        _assessment.Answers.Count(a => a.QuestionCode == "B1").ShouldBe(1);
    }

    [Test]
    public void SetAnswers_StoresMultipleAnswersForOneQuestion()
    {
        _assessment.SetAnswers("D3", ["Dyslexia", "ADHD / ADD"]);

        var answers = _assessment.GetAnswers("D3").ToList();
        answers.Count.ShouldBe(2);
        answers.ShouldContain("Dyslexia");
        answers.ShouldContain("ADHD / ADD");
    }

    [Test]
    public void SetAnswers_ReplacesExistingAnswersForCode()
    {
        _assessment.SetAnswers("D3", ["Dyslexia", "ADHD / ADD"]);
        _assessment.SetAnswers("D3", ["Epilepsy"]);

        var answers = _assessment.GetAnswers("D3").ToList();
        answers.Count.ShouldBe(1);
        answers.ShouldContain("Epilepsy");
    }

    [Test]
    public void SetAnswer_DoesNotAffectOtherQuestions()
    {
        _assessment.SetAnswer("B1", "Sleep rough");
        _assessment.SetAnswer("B2", "Yes");

        _assessment.GetAnswer("B1").ShouldBe("Sleep rough");
        _assessment.GetAnswer("B2").ShouldBe("Yes");
    }

    [Test]
    public void GetAnswer_ReturnsNull_WhenQuestionNotAnswered()
        => _assessment.GetAnswer("B1").ShouldBeNull();

    [Test]
    public void GetAnswers_ReturnsEmpty_WhenQuestionNotAnswered()
        => _assessment.GetAnswers("D3").ShouldBeEmpty();

    [Test]
    public void Answers_ReadOnlyCollection_ReflectsAllStoredAnswers()
    {
        _assessment.SetAnswer("B1", "Sleep rough");
        _assessment.SetAnswers("D3", ["Dyslexia", "ADHD / ADD"]);

        _assessment.Answers.Count.ShouldBe(3);
    }
}
