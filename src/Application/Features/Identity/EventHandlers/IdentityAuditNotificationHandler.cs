namespace Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;

public class IdentityAuditNotificationHandler(IUnitOfWork unitOfWork) : INotificationHandler<IdentityAuditNotification>
{
    public async Task Handle(IdentityAuditNotification notification, CancellationToken cancellationToken)
    {
        IdentityAuditTrail audit = IdentityAuditTrail.Create(notification.UserName, notification.PerformedBy, notification.ActionType, notification.IpAddress);
        await unitOfWork.DbContext.IdentityAuditTrails.AddAsync(audit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
