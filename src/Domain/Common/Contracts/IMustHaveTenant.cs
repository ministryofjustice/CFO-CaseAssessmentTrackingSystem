namespace Cfo.Cats.Domain.Common.Contracts;

public interface IMustHaveTenant
{
    string TenantId { get; set; }
}
