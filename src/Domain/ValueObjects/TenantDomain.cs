using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.ValueObjects;

public class TenantDomain(string domain) : ValueObject, IAuditable
{
    public string Domain { get; private set; } = domain;
    public DateTime? Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Domain;
    }

}
