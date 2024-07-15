using Cfo.Cats.Domain.Common.Contracts;
using Cfo.Cats.Domain.Identity;

namespace Cfo.Cats.Domain.ValueObjects;

public class Note : ValueObject, IAuditable
{
    public required string Message { get; set; }
    public string? CallReference { get; set; }
    public DateTime? Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
    public virtual ApplicationUser? CreatedByUser { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CreatedBy ?? Guid.NewGuid().ToString();
        yield return Created ?? DateTime.Now;
    }
}
