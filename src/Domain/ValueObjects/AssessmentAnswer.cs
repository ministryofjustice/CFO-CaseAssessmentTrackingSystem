namespace Cfo.Cats.Domain.ValueObjects;

public class AssessmentAnswer(string questionCode, string answer) : ValueObject
{
    /// <summary>
    /// The stable code identifying the question (e.g. "B1", "D3", "H13").
    /// </summary>
    public string QuestionCode { get; private set; } = questionCode;

    /// <summary>
    /// One answer value. For single-choice questions there will be exactly one
    /// row per question; for multiple-choice questions there will be one row per
    /// selected answer.
    /// </summary>
    public string Answer { get; private set; } = answer;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return QuestionCode;
        yield return Answer;
    }
}
