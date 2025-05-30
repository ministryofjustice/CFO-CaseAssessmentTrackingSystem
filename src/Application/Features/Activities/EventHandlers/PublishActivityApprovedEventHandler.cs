﻿using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Activities.EventHandlers;

public class PublishActivityApprovedEventHandler(IUnitOfWork unitOfWork) : INotificationHandler<ActivityApprovedDomainEvent>
{
    public async Task Handle(ActivityApprovedDomainEvent notification, CancellationToken cancellationToken)
    {
        await unitOfWork.DbContext.InsertOutboxMessage(new ActivityApprovedIntegrationEvent(notification.Item.Id, notification.DateOccurred.DateTime));
    }
}
