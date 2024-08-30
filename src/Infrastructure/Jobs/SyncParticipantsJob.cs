using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Quartz;
using System.Threading;

namespace Cfo.Cats.Infrastructure.Jobs;

public class SyncParticipantsJob(
    ICandidateService candidateService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<SyncParticipantsJob> logger,
    IUnitOfWork unitOfWork) : IJob
{
    public static readonly JobKey Key = new JobKey(name: nameof(SyncParticipantsJob));
    public static readonly string Description = "A job to synchronise participant information retrieved by the Candidate Service";

    public async Task Execute(IJobExecutionContext context)
    {
        if (context.RefireCount > 3)
        {
            logger.LogWarning($"Quartz Job - {Key}: failed to complete within 3 tries, aborting...");
            return;
        }

        try
        {
            var participants = await unitOfWork.DbContext.Participants
                .IgnoreAutoIncludes() // This doesn't work lol
                .Include(x => x.CurrentLocation)
                .ToListAsync();

            var locations = await unitOfWork.DbContext.Locations.ToListAsync();

            foreach (var participant in participants)
            {
                // Begin transaction
                await unitOfWork.BeginTransactionAsync();

                try
                {
                    // Retrieve up-to-date details from candidate service
                    var candidate = await candidateService.GetByUpciAsync(participant.Id);

                    if (candidate is null)
                    {
                        continue;
                    }

                    // Update and move locations
                    var location = locations.Single(x => x.Id == candidate.MappedLocationId);
                    participant.MoveToLocation(location);

                    // Update external identifiers (Crn, Nomis Number, Pnc Number)
                    if (candidate.Crn is not null)
                    {
                        participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(candidate.Crn, ExternalIdentifierType.Crn));
                    }

                    if (candidate.NomisNumber is not null)
                    {
                        participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(candidate.NomisNumber, ExternalIdentifierType.NomisNumber));
                    }

                    if (candidate.PncNumber is not null)
                    {
                        participant.AddOrUpdateExternalIdentifier(ExternalIdentifier.Create(candidate.PncNumber, ExternalIdentifierType.PncNumber));
                    }

                    // Update first, middle, and last names
                    participant.AddOrUpdateNameInformation(
                        candidate.FirstName, 
                        candidate.SecondName, 
                        candidate.LastName);

                    // Update date of birth
                    participant.AddOrUpdateDateOfBirth(
                        DateOnly.FromDateTime(candidate.DateOfBirth));

                    // Update gender
                    if (candidate.Gender is not null)
                    {
                        participant.AddOrUpdateGender(candidate.Gender);
                    }

                    // Update active in feed status
                    participant.UpdateActiveStatus(candidate.IsActive);

                    // Dispatch events and commit transaction
                    await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
                    await unitOfWork.CommitTransactionAsync();
                }
                catch
                {
                    await unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: $"Quartz Job - {Key}: An unexpected error occurred executing job", refireImmediately: true, cause: ex);
        }

    }

}
