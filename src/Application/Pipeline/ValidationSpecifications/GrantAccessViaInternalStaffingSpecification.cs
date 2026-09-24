namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class GrantAccessViaInternalStaffingSpecification(ICurrentUserService currentUserService) : AccessValidationSpecification
{
    public override int Order => 0;

    protected override Task<AccessGrant> CheckAccessAsync(string identifier) => 
        Task.FromResult(currentUserService.TenantId is "1." or "1.1." ? AccessGrant.Granted : AccessGrant.NotGranted);
}
