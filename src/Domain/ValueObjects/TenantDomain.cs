namespace Cfo.Cats.Domain.ValueObjects;

public class TenantDomain(string domain) : ValueObject
{
    public string Domain { get; private set; } = domain;

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Domain;
    }

}
