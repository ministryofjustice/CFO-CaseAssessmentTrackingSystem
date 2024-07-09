using Cfo.Cats.Domain.Common.Contracts;

namespace Cfo.Cats.Domain.ValueObjects;

public class Note(string message, string? callReference) : ValueObject, IAuditable
{
    public string Message { get; private set; } = message;
    public string? CallReference { get; private set; } = callReference;
    public DateTime? Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CreatedBy ?? Guid.NewGuid().ToString();
        yield return Created ?? DateTime.Now;
    }
}
