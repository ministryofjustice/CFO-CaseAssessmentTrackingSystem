namespace Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;

public class IdentityAuditNotificationHandler(IUnitOfWork unitOfWork) : INotificationHandler<IdentityAuditNotification>
{
    public async Task Handle(IdentityAuditNotification notification, CancellationToken cancellationToken)
    {
        IdentityAuditTrail audit = IdentityAuditTrail.Create(notification.UserName, notification.PerformedBy, notification.ActionType, notification.IpAddress);
        unitOfWork.DbContext.IdentityAuditTrails.Add(audit);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        switch (notification.ActionType)
        {
            case IdentityActionType.LoginPasswordOnly:
            case IdentityActionType.LoginWithTwoFactorCode:                 
                await SaveLoginDetails(notification.UserName, cancellationToken);
                break;
        }
    }

    private async Task SaveLoginDetails(string? userName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(userName))
        {
            throw new ArgumentException("Username cannot be null or empty", nameof(userName));
        }

        var user = await unitOfWork.DbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

        if (user == null)
        {            
            throw new InvalidOperationException($"User with username '{userName}' not found.");
        }

        user.LastLogin = DateTime.UtcNow;

        await unitOfWork.SaveChangesAsync(cancellationToken);        
    }
}