using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Participants;
using Quartz;

namespace Cfo.Cats.Infrastructure.Jobs;

public class SyncParticipantsJob(
    ICandidateService candidateService,
    IDomainEventDispatcher domainEventDispatcher,
    ILogger<SyncParticipantsJob> logger,
    IUnitOfWork unitOfWork) : IJob
{

    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);


    public static readonly JobKey Key = new JobKey(name: nameof(SyncParticipantsJob));
    public static readonly string Description = "A job to synchronise participant information retrieved by the Candidate Service";

    public async Task Execute(IJobExecutionContext context)
    {
        if (context.RefireCount > 3)
        {
            logger.LogWarning($"Quartz Job - {Key}: failed to complete within 3 tries, aborting...");
            return;
        }

        if (!await _semaphore.WaitAsync(TimeSpan.Zero))
        {
            // Job is already running, skip this execution
            return;
        }

        using var outScope = logger.BeginScope($"Starting job {Key}. Refire count {context.RefireCount}");

        try
        {
            var participants = await unitOfWork.DbContext.Participants
                .IgnoreAutoIncludes()
                .Include(x => x.CurrentLocation)
                .ToListAsync();

            var locations = await unitOfWork.DbContext.Locations.ToListAsync();

            foreach (var participant in participants)
            {
                // Begin transaction
                await unitOfWork.BeginTransactionAsync();

                using var scope = logger.BeginScope("Sync for Participant: {Id}", [participant.Id]);

                try
                {
                    // Retrieve up-to-date details from candidate service
                    logger.LogTrace($"Retrieving candidate information");
                    var candidate = await candidateService.GetByUpciAsync(participant.Id);

                    if (candidate is null)
                    {
                        logger.LogWarning("No information found");
                        continue;
                    }

                    // Update and move locations
                    logger.LogTrace("Update location");
                    var location = locations.Single(x => x.Id == candidate.MappedLocationId);
                    participant.MoveToLocation(location);

                    // Update external identifiers (Crn, Nomis Number, Pnc Number)
                    logger.LogTrace("Update external identifier(s)");
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
                    logger.LogTrace("Update name(s)");
                    participant.UpdateNameInformation(
                        candidate.FirstName, 
                        candidate.SecondName, 
                        candidate.LastName);

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

                    // Dispatch events and commit transaction
                    await domainEventDispatcher.DispatchEventsAsync(unitOfWork.DbContext, CancellationToken.None);
                    await unitOfWork.CommitTransactionAsync();
                }
                catch(Exception e)
                {
                    logger.LogError(e, e.Message);
                    await unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: $"Quartz Job - {Key}: An unexpected error occurred executing job",
                refireImmediately: true, cause: ex);
        }
        finally
        {
            _semaphore.Release();
        }

        logger.LogInformation($"Job {Key} completed");

    }

}
