namespace Cfo.Cats.Domain.Common.Contracts;

public interface IMayHaveTenant
{
    string? TenantId { get; set; }
}
