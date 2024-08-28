using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cfo.Cats.Application.Features.Identity.Notifications.IdentityEvents;

public class IdentityAuditNotificationHandler(IUnitOfWork unitOfWork) : INotificationHandler<IdentityAuditNotification>
{
    public async Task Handle(IdentityAuditNotification notification, CancellationToken cancellationToken)
    {
        IdentityAuditTrail audit = IdentityAuditTrail.Create(notification.UserName, notification.PerformedBy, notification.ActionType);
        unitOfWork.DbContext.IdentityAuditTrails.Add(audit);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}