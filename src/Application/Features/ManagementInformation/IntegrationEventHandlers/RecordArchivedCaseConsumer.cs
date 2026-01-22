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
            data.Created,
            data.CreatedBy,
            data.AdditionalInfo,
            data.ArchiveReason,
            data.From,
            data.ContractId,
            data.LocationId,
            data.LocationType,
            data.TenantId
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
                Created = context.OccuredOn,
                CreatedBy = peh.CreatedBy!,
                AdditionalInfo = peh.AdditionalInformation!,
                ArchiveReason = peh.Reason,
                From = peh.From,
                ContractId = l.Contract!.Id,
                LocationId = l.Id,
                LocationType = l.LocationType.Name,
                TenantId = u.TenantId!
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
        public required DateTime Created { get; set; }
        public required string CreatedBy { get; set; }
        public required string? AdditionalInfo { get; set; }
        public required string? ArchiveReason { get; set; }
        public required DateTime From { get; set; }
        public required string ContractId { get; set; }
        public required int LocationId { get; set; }
        public required string LocationType { get; set; }
        public required string TenantId { get; set; }
    }
}