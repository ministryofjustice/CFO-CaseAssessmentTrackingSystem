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
            data.FirstName,
            data.LastName,
            data.EnrolmentHistoryId,
            data.Created,
            data.CreatedBy,
            data.ArchiveAdditionalInfo,
            data.ArchiveReason,
            data.UnarchiveAdditionalInfo,
            data.UnarchiveReason,
            data.From,
            data.To,
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
                  && peh.To == null 
        
            select new Data
            {
                ParticipantId = context.ParticipantId,
                FirstName = p.FirstName,
                LastName = p.LastName,
                EnrolmentHistoryId = peh.Id,
                Created = context.OccuredOn,
                CreatedBy = u.CreatedBy!,
                ArchiveAdditionalInfo = peh.AdditionalInformation!,
                ArchiveReason = peh.Reason,
                UnarchiveAdditionalInfo = null,
                UnarchiveReason = null,
                From = peh.From,
                To = peh.To,
                ContractId = l.Contract!.Id,
                LocationId = l.Id,
                LocationType = l.LocationType.Name,
                TenantId = u.TenantId!
            };
        
        var data = await query
            .OrderByDescending(x => x.From)
            .Take(1)
            .SingleOrDefaultAsync();
    
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
        public required string FirstName { get; set; }
        public required string LastName { get;  set; }
        public required int EnrolmentHistoryId { get; set; }
        public required DateTime Created { get; set; }
        public required string CreatedBy { get; set; }
        public required string? ArchiveAdditionalInfo { get; set; }
        public required string? ArchiveReason { get; set; }
        public required string? UnarchiveAdditionalInfo { get; set; }
        public required string? UnarchiveReason { get; set; }
        public required DateTime From { get; set; }
        public required DateTime? To { get; set; }
        public required string ContractId { get; set; }
        public required int LocationId { get; set; }
        public required string LocationType { get; set; }
        public required string TenantId { get; set; }
    }
}