namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public class GrantAccessViaOpenPriSpecification(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : AccessValidationSpecification
{
    public override int Order => 1;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier)
    {
        var pri = await unitOfWork.DbContext.PRIs
                    .Where(p => p.ParticipantId == identifier)
                    .Where(p => p.CompletedOn == null)
                    .Select(p => p.AssignedTo)
                    .FirstOrDefaultAsync();

        return pri switch
        {
            null => AccessGrant.NotGranted,
            string p when p == currentUserService.UserId => AccessGrant.Granted,
            _ => AccessGrant.NotGranted
        };
    }
}