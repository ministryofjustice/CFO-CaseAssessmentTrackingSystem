namespace Cfo.Cats.Application.Pipeline.ValidationSpecifications;

public sealed class CurrentUserWasOwnerRecentlyAccessSpecification(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : AccessValidationSpecification
{
    public override int Order => 10;

    protected override async Task<AccessGrant> CheckAccessAsync(string identifier) =>              
        (await unitOfWork.DbContext.ParticipantOwnershipHistories
                 .Where(h => h.ParticipantId == identifier)
                 .Where(h => h.OwnerId == currentUserService.UserId)
                 .Where(h => h.From >= DateTime.Now.AddMonths(-3).Date)
                 .AnyAsync()) ? AccessGrant.Granted : AccessGrant.NotGranted;

}