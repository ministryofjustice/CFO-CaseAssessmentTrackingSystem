using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Application.Messaging;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class CloseOffLastArchivedCaseEntry(IUnitOfWork unitOfWork) : IIntegrationEventHandler<ParticipantTransitionedIntegrationEvent>
{
    public async Task Handle(ParticipantTransitionedIntegrationEvent context, CancellationToken cancellationToken)
    {
        if (context.From != EnrolmentStatus.ArchivedStatus.Name)
        {
            return;
        }
        
        var db = unitOfWork.DbContext;

        var archivedCase = await db.ArchivedCases
            .Where(ac =>
                ac.ParticipantId == context.ParticipantId 
                && ac.To == null)
            .OrderByDescending(ac => ac.From)
            .FirstOrDefaultAsync();
        
        if (archivedCase is null)
        {
            return;
        }
        
        var unarchiveHistory = await db.ParticipantEnrolmentHistories
            .Where(peh =>
                peh.ParticipantId == context.ParticipantId &&
                peh.From >= archivedCase.From &&
                peh.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value)
            .OrderBy(peh => peh.From)
            .FirstOrDefaultAsync();

        archivedCase.Close(
            context.OccuredOn,
            unarchiveHistory?.AdditionalInformation,
            unarchiveHistory?.Reason
            
        );

        await unitOfWork.SaveChangesAsync();
    }
}