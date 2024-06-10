namespace Cfo.Cats.Domain.Common.Contracts;

public interface IAuditableEntity
{
    DateTime? Created { get; set; }

    string? CreatedBy { get; set; }

    DateTime? LastModified { get; set; }

    string? LastModifiedBy { get; set; }
}
