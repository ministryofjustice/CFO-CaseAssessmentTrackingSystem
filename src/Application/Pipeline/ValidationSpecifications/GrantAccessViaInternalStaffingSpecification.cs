namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class GrantAccessViaInternalStaffingSpecification(ICurrentUserService currentUserService) : AccessValidationSpecification
{
    public override int Order => 0;

    protected override Task<AccessGrant> CheckAccessAsync(string identifier, CancellationToken cancellationToken) =>
        Task.FromResult(currentUserService.IsInternalUser.GetValueOrDefault() ? AccessGrant.Granted : AccessGrant.NotGranted);

    protected override Task<IReadOnlyDictionary<string, AccessGrant>> CheckAccessManyAsync(
        IReadOnlyCollection<string> identifiers,
        CancellationToken cancellationToken)
    {
        var grant = currentUserService.IsInternalUser.GetValueOrDefault() ? AccessGrant.Granted : AccessGrant.NotGranted;
        IReadOnlyDictionary<string, AccessGrant> results = identifiers.ToDictionary(id => id, _ => grant, StringComparer.OrdinalIgnoreCase);
        return Task.FromResult(results);
    }
}
