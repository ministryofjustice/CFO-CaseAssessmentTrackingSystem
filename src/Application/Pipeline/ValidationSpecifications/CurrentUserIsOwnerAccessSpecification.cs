namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class CurrentUserIsOwnerAccessSpecification(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : AccessValidationSpecification
{
    public override int Order => 1;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier) =>
         (await unitOfWork.DbContext.Participants
                    .Where(p => p.Id == identifier)
                    .Where(p => p.OwnerId == currentUserService.UserId)
                    .AnyAsync()) ? AccessGrant.Granted : AccessGrant.NotGranted;
       
}
