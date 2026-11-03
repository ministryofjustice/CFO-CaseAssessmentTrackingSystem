namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class GrantAccessViaInternalStaffingSpecification(ICurrentUserService currentUserService) : AccessValidationSpecification
{
    public override int Order => 0;

    protected override Task<AccessGrant> CheckAccessAsync(string identifier, CancellationToken cancellationToken) =>
        Task.FromResult(currentUserService.IsInternalUser.GetValueOrDefault() ? AccessGrant.Granted : AccessGrant.NotGranted);
}
