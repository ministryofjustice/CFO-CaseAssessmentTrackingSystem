namespace Cfo.Cats.Domain.ValueObjects;

public class PathwayScore(string pathway, double score) : ValueObject
{

    /// <summary>
    /// The name of the pathway
    /// </summary>
    public string Pathway { get; private set;} = pathway;

    /// <summary>
    /// The calculated scores
    /// </summary>
    public double Score {get; private set;} = score;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Pathway;
        yield return Score;
    }
}
