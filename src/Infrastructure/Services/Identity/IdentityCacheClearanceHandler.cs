using Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;
using MediatR;
using ZiggyCreatures.Caching.Fusion;

namespace Cfo.Cats.Infrastructure.Services.Identity;

public class IdentityCacheClearanceHandler(IUnitOfWork unitOfWork, IFusionCache cache) :   INotificationHandler<IdentityAuditNotification>
{
    public async Task Handle(IdentityAuditNotification notification, CancellationToken cancellationToken)
    {
        var user = await unitOfWork.DbContext
            .Users
            .Where(u => u.UserName == notification.UserName)
            .Select(u => u.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is not null)
        {
            var key = ApplicationUserClaimsPrincipalFactory.GetCacheKey(user);
            await cache.RemoveAsync(key, token: cancellationToken);
        }
    }
}