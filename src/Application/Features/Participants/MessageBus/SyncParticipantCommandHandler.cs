using Cfo.Cats.Domain.Entities.Participants;
using Rebus.Handlers;


namespace Cfo.Cats.Application.Features.Participants.MessageBus;

public class SyncParticipantCommandHandler(
    IUnitOfWork unitOfWork,
    ICandidateService candidateService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<SyncParticipantCommandHandler> logger)
    : IHandleMessages<SyncParticipantCommand>
{
    public async Task Handle(SyncParticipantCommand context)
    {
        var participant = await unitOfWork.DbContext
            .Participants
            .IgnoreAutoIncludes()
            .Include(x => x.CurrentLocation)
            .AsSplitQuery()
            .FirstAsync(x => x.Id == context.ParticipantId);
    
        logger.LogDebug($"Syncing {participant.Id}");

        try
        {
            using var scope = logger.BeginScope("Sync for Participant: {ParticipantId}", participant.Id);

            var result = await candidateService.GetByUpciAsync(participant.Id);

            if (result.Succeeded == false)
            {
                throw new InvalidOperationException($"Error retrieving DMS information: {result.ErrorMessage}");
            }

            var candidate = result.Data!;

            await unitOfWork.BeginTransactionAsync();

            var location = await unitOfWork.DbContext.Locations.SingleAsync(x => x.Id == candidate.MappedLocationId);
            participant.MoveToLocation(location);

            if (candidate.Crn is not null)
            {
                participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(candidate.Crn.ToUpper(),
                    ExternalIdentifierType.Crn));
            }

            if (candidate.NomisNumber is not null)
            {
                participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(candidate.NomisNumber.ToUpper(),
                    ExternalIdentifierType.NomisNumber));
            }

            if (candidate.PncNumber is not null)
            {
                participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(candidate.PncNumber.ToUpper(),
                    ExternalIdentifierType.PncNumber));
            }

            // Update first, middle, and last names
            logger.LogTrace("Update name(s)");
            participant.UpdateNameInformation(
                candidate.FirstName.ToUpper(),
                candidate.SecondName?.ToUpper(),
                candidate.LastName.ToUpper());

            // Update date of birth
            logger.LogTrace("Update date of birth");
            participant.UpdateDateOfBirth(DateOnly.FromDateTime(candidate.DateOfBirth));

            // Update gender
            logger.LogTrace("Update gender");
            participant.UpdateGender(candidate.Gender);

            // Update active in feed status
            logger.LogTrace("Update active status");
            participant.UpdateActiveStatus(candidate.IsActive);

            // Update registration details json
            logger.LogTrace("Update registration details");
            participant.UpdateRegistrationDetailsJson(candidate.RegistrationDetailsJson);

            logger.LogTrace("Update nationality");
            participant.UpdateNationality(candidate.Nationality);

            participant.UpdateSync();

            // Dispatch events and commit transaction
            await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
            await unitOfWork.CommitTransactionAsync();
            logger.LogDebug("Finished syncing {ParticipantId}", participant.Id);
        }

        catch (Exception e)
        {
            logger.LogError(e, "Failed to sync participant {ParticipantId}", context.ParticipantId);
            await unitOfWork.RollbackTransactionAsync();
        }
           
            
    }
}