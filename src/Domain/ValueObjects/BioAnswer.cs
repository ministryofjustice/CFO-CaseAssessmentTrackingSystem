namespace Cfo.Cats.Domain.ValueObjects;

public class BioAnswer(string questionCode, string answer) : ValueObject
{
    /// <summary>
    /// The stable code identifying the question (e.g. "A1", "B2", "C3").
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
