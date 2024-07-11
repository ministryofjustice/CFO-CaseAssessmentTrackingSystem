﻿using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Timelines.EventHandlers;

public abstract class TimelineNotificationHandler<TDomainEvent>(ICurrentUserService currentUserService, IApplicationDbContext context) : INotificationHandler<TDomainEvent>
    where TDomainEvent : INotification

{

    public async Task Handle(TDomainEvent notification, CancellationToken cancellationToken)
    {
        var timelineEvent = Timeline.CreateTimeline(
        GetParticipantId(notification),
        GetEventType(),
        currentUserService.UserId!,
        GetLine1(notification),
        GetLine2(notification),
        GetLine3(notification));

        context.Timelines.Add(timelineEvent);
        await context.SaveChangesAsync(cancellationToken);
    }


    protected abstract string GetLine1(TDomainEvent notification);
    protected virtual string? GetLine2(TDomainEvent notification) => null;
    protected virtual string? GetLine3(TDomainEvent notification) => null;
    protected abstract TimelineEventType GetEventType();

    protected abstract string GetParticipantId(TDomainEvent notification);
}