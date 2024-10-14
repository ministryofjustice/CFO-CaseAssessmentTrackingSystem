namespace Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;

public class UpdateLastUpdatedStampNotificationHandler(IUnitOfWork unitOfWork) : INotificationHandler<IdentityAuditNotification>
{
    public async Task Handle(IdentityAuditNotification notification, CancellationToken cancellationToken)
    {
        IdentityActionType[] types = [ IdentityActionType.LoginPasswordOnly, IdentityActionType.LoginWithTwoFactorCode ];

        if(types.Contains(notification.ActionType))
        {
            await unitOfWork.DbContext.Users
                    .Where(u => u.UserName == notification.UserName)
                    .ExecuteUpdateAsync(e => e.SetProperty(u => u.LastLogin, DateTime.UtcNow));
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}