using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Outbox;
using Cfo.Cats.Domain.Entities.Bios;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.ManagementInformation.EventHandlers;

public class PublishBioEngagementEventHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService) : 
    INotificationHandler<BioSubmittedDomainEvent>
{
    public async Task Handle(BioSubmittedDomainEvent notification, CancellationToken cancellationToken)
        => await Handle(notification.Item, notification.DateOccurred.DateTime, "Submitted");

    private async Task Handle(ParticipantBio assessment, DateTime EngagedOn, string engagementType)
    {
        var e = new ParticipantEngagedIntegrationEvent(
            ParticipantId: assessment.ParticipantId,
            Description: $"{engagementType}: {assessment.Status.Name}",
            Category: "Bio",
            EngagedOn: DateOnly.FromDateTime(EngagedOn),
            UserId: currentUserService.UserId!,
            TenantId: currentUserService.TenantId!);

        await unitOfWork.DbContext.InsertOutboxMessage(e);
    }
}
