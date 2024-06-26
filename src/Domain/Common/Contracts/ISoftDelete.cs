namespace Cfo.Cats.Domain.Common.Contracts;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }
    string? DeletedBy { get; set; }
}
