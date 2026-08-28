namespace Cfo.Cats.Domain.Common.Contracts;

public interface IAuditable
{
    DateTime? Created { get; set; }

    string? CreatedBy { get; set; }

    DateTime? LastModified { get; set; }

    string? LastModifiedBy { get; set; }
}
/// <summary>
/// This indicates that the auditable entity should only care about Created and LastModified
/// and does not need to do a deep audit.
/// 
/// Introduced as a less dangerous way to audit, so you have to be explicit on which auditable
/// entities you need to not keep masses of data on, but still want the created flags.
/// </summary>
public interface IShallowAuditable : IAuditable
{

}