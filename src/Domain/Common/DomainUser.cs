namespace Cfo.Cats.Domain.Common;

public sealed record DomainUser(string Id, string UserName, string TenantId, bool IsInternalUser);
