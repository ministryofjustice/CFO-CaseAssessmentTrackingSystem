
namespace Cfo.Cats.Domain.Entities.Inductions;

public class InductionPhase(int number, DateTime startDate, DateTime? completedDate) : ValueObject
{
    public int Number { get; private set; } = number;

    public DateTime StartDate { get; private set; } = startDate;
    
    public DateTime? CompletedDate { get; private set; } = completedDate;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }
}
