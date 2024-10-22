﻿using Cfo.Cats.Domain.Entities.Participants;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Participants.EventHandlers;

public class ParticipantAssigned(IUnitOfWork unitOfWork) : INotificationHandler<ParticipantAssignedDomainEvent>
{
    public async Task Handle(ParticipantAssignedDomainEvent notification, CancellationToken cancellationToken)
    {
        var history = ParticipantOwnershipHistory.Create(
            notification.Item.Id, 
            notification.NewOwner!, 
            DateTime.UtcNow);

        await unitOfWork.DbContext.ParticipantOwnershipHistories.AddAsync(history, cancellationToken);
    }

}
