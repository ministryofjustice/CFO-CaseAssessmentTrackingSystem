using Cfo.Cats.Domain.Entities.Participants;
using MassTransit;

namespace Cfo.Cats.Application.Features.Participants.MessageBus;

[ExcludeFromConfigureEndpoints]
public class SyncParticipantCommandHandler(IUnitOfWork unitOfWork, ICandidateService candidateService, IDomainEventDispatcher domainEventDispatcher, ILogger<SyncParticipantCommandHandler> logger) 
    : IConsumer<SyncParticipantCommand>
{
    public async Task Consume(ConsumeContext<SyncParticipantCommand> context)
    {
        var participant = await unitOfWork.DbContext
            .Participants
            .IgnoreAutoIncludes()
            .Include(x => x.CurrentLocation)
            .AsSplitQuery()
            .FirstAsync(x => x.Id == context.Message.ParticipantId);
    
        logger.LogDebug($"Syncing {participant.Id}");

        try
        {
            using var scope = logger.BeginScope("Sync for Participant: {Id}", [participant.Id]);

            var candidate = await candidateService.GetByUpciAsync(participant.Id);

            if (candidate is null)
            {
                throw new InvalidOperationException("No DMS information found");
            }

            await unitOfWork.BeginTransactionAsync();

            var location = await unitOfWork.DbContext.Locations.SingleAsync(x => x.Id == candidate.MappedLocationId);
            participant.MoveToLocation(location);

            if (candidate.Crn is not null)
            {
                participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(candidate.Crn,
                    ExternalIdentifierType.Crn));
            }

            if (candidate.NomisNumber is not null)
            {
                participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(candidate.NomisNumber,
                    ExternalIdentifierType.NomisNumber));
            }

            if (candidate.PncNumber is not null)
            {
                participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(candidate.PncNumber,
                    ExternalIdentifierType.PncNumber));
            }

            // Update first, middle, and last names
            logger.LogTrace("Update name(s)");
            participant.UpdateNameInformation(
                candidate.FirstName.ToTitleCase(),
                candidate.SecondName?.ToTitleCase(),
                candidate.LastName.ToTitleCase());

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

        }

        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            await unitOfWork.RollbackTransactionAsync();
            throw;
        }
        finally
        {
            logger.LogDebug($"Finished syncing {participant.Id}");
        }    

            
            
    }
}