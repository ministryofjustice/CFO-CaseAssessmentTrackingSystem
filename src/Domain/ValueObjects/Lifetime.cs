using Cfo.Cats.Domain.Common.Exceptions;

namespace Cfo.Cats.Domain.ValueObjects;

public class Lifetime : ValueObject 
{
    public Lifetime(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            throw new InvalidLifetimeException();
        }
        
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    public Lifetime Extend(DateTime to)
    {
        return new Lifetime(this.StartDate, to);
    }
}