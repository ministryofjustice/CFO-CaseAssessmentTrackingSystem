using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.ValueObjects;

public class TenantDomain(string domain) : ValueObject, IAuditable
{
    public string Domain { get; private set; } = domain;

    [AuditIgnore]
    public DateTime? Created { get; set; }

    [AuditIgnore]
    public string? CreatedBy { get; set; }

    [AuditIgnore]
    public DateTime? LastModified { get; set; }

    [AuditIgnore]
    public string? LastModifiedBy { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Domain;
    }

}
