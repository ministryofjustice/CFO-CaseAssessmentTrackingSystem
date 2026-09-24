namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class GrantAccessViaOwnershipTree(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : AccessValidationSpecification
{
    public override int Order => 10;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier)
    {
        var cutoff = DateTime.Today.AddMonths(-3);
        
        var owners = unitOfWork.DbContext.ParticipantOwnershipHistories
                        .Where(h => h.ParticipantId == identifier)
                        .Where(h => h.To == null || h.To >= cutoff)
                        .Select(h => h.TenantId!)
                        .Distinct();

        return owners.Any(x => x.StartsWith(currentUserService.TenantId!)) ? AccessGrant.Granted : AccessGrant.NotGranted;

    }
                 
}