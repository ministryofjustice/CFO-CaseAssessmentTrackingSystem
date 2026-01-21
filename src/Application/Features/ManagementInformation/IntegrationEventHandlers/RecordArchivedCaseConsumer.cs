using Cfo.Cats.Application.Features.Participants.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordArchivedCaseConsumer(IUnitOfWork unitOfWork)
    : IHandleMessages<ParticipantTransitionedIntegrationEvent>
{
    public async Task Handle(ParticipantTransitionedIntegrationEvent context)
    {
        if (context.To != EnrolmentStatus.ArchivedStatus.Name)
        {
            return;
        }

        var data = await GetData(context);

        var archivedCase = CreateArchivedCase(data);

        unitOfWork.DbContext.ArchivedCases.Add(archivedCase);
        await unitOfWork.SaveChangesAsync();
    }

    private static ArchivedCase CreateArchivedCase(Data data)
        => ArchivedCase.CreateArchivedCase(
            data.ParticipantId,
            data.EnrolmentHistoryId,
            data.OccurredOn,
            data.TenantId,
            data.SupportWorker,
            data.ContractId,
            data.LocationId,
            data.LocationType,
            data.ArchiveReason
        );

    private async Task<Data> GetData(ParticipantTransitionedIntegrationEvent context)
    {
        var db = unitOfWork.DbContext;

        var query =
            from p in db.Participants
            join peh in db.ParticipantEnrolmentHistories on p.Id equals peh.ParticipantId
            join u in db.Users on peh.CreatedBy equals u.Id
            from lh in db.ParticipantLocationHistories
                .Where(lh =>
                    lh.ParticipantId == p.Id &&
                    peh.From >= lh.From &&
                    (lh.To == null || peh.From <= lh.To))
                .OrderByDescending(lh => lh.From)
                .Take(1)
            join l in db.Locations on lh.LocationId equals l.Id

            where p.Id == context.ParticipantId
                  && peh.EnrolmentStatus == EnrolmentStatus.ArchivedStatus.Value
                  && peh.From == context.OccuredOn
                  && peh.From >= lh.From
                  && (peh.To == null || peh.To <= lh.To)

            select new Data
            {
                ParticipantId = context.ParticipantId,
                EnrolmentHistoryId = peh.Id,
                TenantId = u.TenantId!,
                SupportWorker = peh.CreatedBy!,
                ContractId = l.Contract!.Id,
                LocationId = l.Id,
                LocationType = l.LocationType.Name,
                OccurredOn = context.OccuredOn,
                CreatedOn = peh.Created!.Value,
                ArchiveReason = peh.Reason
            };

        var data = await query.SingleOrDefaultAsync();

        if (data is null)
        {
            throw new InvalidOperationException(
                $"No archived enrolment found for participant {context.ParticipantId}");
        }

        return data;
    }

    public record Data
    {
        public required string ParticipantId { get; set; }
        public required int EnrolmentHistoryId { get; set; }
        public required string TenantId { get; set; }
        public required string SupportWorker { get; set; }
        public required string ContractId { get; set; }
        public required int LocationId { get; set; }
        public required string LocationType { get; set; }

        public required DateTime OccurredOn { get; set; }
        public required DateTime CreatedOn { get; set; }

        public required string? ArchiveReason { get; set; }
    }
}