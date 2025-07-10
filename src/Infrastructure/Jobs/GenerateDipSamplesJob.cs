using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Quartz;
using System.Linq.Dynamic.Core;

namespace Cfo.Cats.Infrastructure.Jobs;

public class GenerateDipSamplesJob(
    ILogger<GenerateDipSamplesJob> logger,
    IUnitOfWork unitOfWork) : IJob
{
    public static readonly JobKey Key = new(name: nameof(GenerateDipSamplesJob));
    public static readonly string Description = "A job to generate dip samples for quality checks.";

    public async Task Execute(IJobExecutionContext context)
    {
        using (logger.BeginScope(Key))

        if (context.RefireCount > 3)
        {
            logger.LogWarning($"Failed to complete within 3 tries, aborting...");
            return;
        }

        try
        {
            logger.LogInformation("Generate dip samples");

            var period = DateTime.Today.AddMonths(-4);

            DateTime periodFrom = new(period.Year, period.Month, 1); // i.e. 01/01/1999 00:00:00
            DateTime periodTo = periodFrom.AddMonths(1).AddTicks(-1); // i.e. 31/01/1999 23:59:59

            var query =
                from ep in unitOfWork.DbContext.EnrolmentPayments
                join t in unitOfWork.DbContext.ParticipantOutgoingTransferQueue on ep.ParticipantId equals t.ParticipantId into tj
                from sub in tj.DefaultIfEmpty()
                join p in unitOfWork.DbContext.Participants on ep.ParticipantId equals p.Id
                where
                    p.EnrolmentStatus != EnrolmentStatus.ArchivedStatus.Value
                    && ep.EligibleForPayment
                    && ep.Approved >= periodFrom
                    && ep.Approved <= periodTo
                    && sub == null // Exclude cross-contract transfers
                select new { ep.ParticipantId, ep.LocationId, ep.LocationType, ep.ContractId };

            var enrolments = await query
                .AsNoTracking()
                .ToListAsync();

            List<Sample> samples = [];

            foreach (var contractGroup in enrolments.GroupBy(p => p.ContractId))
            {
                var samplesByLocationType = contractGroup
                    .GroupBy(g => Group(g.LocationType, g.LocationId))
                    .Select(g =>
                    {
                        // Move to config
                        int sampleSize = g.Key.LocationType switch
                        {
                            var type when type == LocationType.Wing.Name => 10,
                            var type when type == LocationType.Hub.Name => 10,
                            var type when type == LocationType.Satellite.Name => 5,
                            var type when type == LocationType.Community.Name => 10,
                            var type when type == "Wider Custody" => 10,
                            _ => 0
                        };

                        return new ContractLocationTypeSample(
                            g.Key.LocationType,
                            g.Key.LocationId,
                            g.OrderBy(x => Random.Shared.Next())
                             .Select(x => x.ParticipantId)
                             .Distinct()
                             .Take(sampleSize));
                    })
                    .ToList();

                samples.Add(new Sample(contractGroup.Key, samplesByLocationType));
            }

            if (samples.Count is 0)
            {
                throw new Exception("No dip samples could be generated.");
            }

            await using var transaction = await unitOfWork.DbContext.Database.BeginTransactionAsync();

            try
            {
                var dipSamples = samples.Select(sample => new
                {
                    Value = sample,
                    Entity = DipSample.Create(sample.ContractId, periodFrom, periodTo)
                }).ToList();

                await unitOfWork.DbContext.DipSamples.AddRangeAsync(dipSamples.Select(x => x.Entity));

                var dipSampleParticipants = dipSamples.SelectMany(participant =>
                    participant.Value.Samples.SelectMany(locationSample =>
                        locationSample.ParticipantIds.Select(pid =>
                            new DipSampleParticipant
                            {
                                DipSampleId = participant.Entity.Id,
                                ParticipantId = pid,
                                LocationType = locationSample.LocationType
                            })));

                await unitOfWork.DbContext.DipSampleParticipants.AddRangeAsync(dipSampleParticipants);
                
                await unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Failed to save dip samples and participants. Transaction rolled back.");
                throw;
            }
        }
        catch (Exception ex)
        {
            throw new JobExecutionException(msg: $"An unexpected error occurred executing job", refireImmediately: true, cause: ex);
        }
    }

    static (string LocationType, int? LocationId) Group(string locationType, int locationId) => 
        new[] { LocationType.Female.Name, LocationType.Feeder.Name,LocationType.Outlying.Name}
        .Contains(locationType) ? ("Wider Custody", null) : (locationType, locationId);


    record ContractLocationTypeSample(string LocationType, int? LocationId, IEnumerable<string> ParticipantIds);
    record Sample(string ContractId, List<ContractLocationTypeSample> Samples);
}