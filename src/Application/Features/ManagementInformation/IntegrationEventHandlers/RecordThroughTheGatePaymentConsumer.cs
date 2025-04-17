using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordThroughTheGatePaymentConsumer(IUnitOfWork unitOfWork)
    : IConsumer<PRIThroughTheGateCompletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PRIThroughTheGateCompletedIntegrationEvent> context)
    {
        var data = await GetData(context.Message.PRIId);

        var payment = GeneratePayment(data);

        unitOfWork.DbContext.SupportAndReferralPayments.Add(payment);
        await unitOfWork.SaveChangesAsync();
    }

    public static SupportAndReferralPayment GeneratePayment(Data data)
    {
        var ineligibilityReason = data switch
        {
            { DateOfFirstConsent: null } => IneligibilityReason.NotYetApproved,
            { CountOfPayments: > 0 } => IneligibilityReason.MaximumPaymentLimitReached,
            { MeetingTookPlaceOnOrAfterConsent: false } => IneligibilityReason.BeforeConsent,
            { AllTasksCompleted: false } => IneligibilityReason.RequiredTasksNotCompleted,
            { TaskTwoCompletedWithin4Weeks: false } => IneligibilityReason.RequiredTasksNotCompletedInTime("4 weeks"),
            { ParticipantInCommunityLocationAfterCustodyLocation: false } => IneligibilityReason.NeverInLocation,
            _ => null
        };

        var payment = ineligibilityReason switch
        {
            null => CreatePayable(data),
            _ => CreateNonPayable(data, ineligibilityReason)
        };

        return payment;
    }

    private static SupportAndReferralPayment CreatePayable(Data data) =>
        SupportAndReferralPayment.CreatePayable(
            priId: data.PriId,
            participantId: data.ParticipantId,
            supportType: "Through the Gate",
            activityInput: DateOnly.FromDateTime(data.ActivityInput),
            approvedDate: DateOnly.FromDateTime(data.Approved),
            supportWorker: data.SupportWorker,
            contractId: data.ContractId,
            locationType: data.LocationType,
            locationId: data.CommunityLocationId,
            tenantId: data.TenantId,
            paymentPeriod: data.PaymentPeriod
        );

    private static SupportAndReferralPayment CreateNonPayable(Data data, IneligibilityReason reason)
        => SupportAndReferralPayment.CreateNonPayable(priId: data.PriId,
            participantId: data.ParticipantId,
            supportType: "Through the Gate",
            activityInput: DateOnly.FromDateTime(data.ActivityInput),
            approvedDate: DateOnly.FromDateTime(data.Approved),
            supportWorker: data.SupportWorker,
            contractId: data.ContractId,
            locationType: data.LocationType,
            locationId: data.CommunityLocationId,
            tenantId: data.TenantId,
            paymentPeriod: data.PaymentPeriod,
            reason: reason);


    private async Task<Data> GetData(Guid priId)
    {
        var db = unitOfWork.DbContext;

        var query = from p in db.Participants
            join pri in db.PRIs on p.Id equals pri.ParticipantId
            join l in db.Locations on pri.ExpectedReleaseRegionId equals l.Id
            join u in db.Users on pri.AssignedTo equals u.Id
            where pri.Id == priId
            select new Data
            {
                PriId = pri.Id,
                ActivityInput = pri.Created!.Value,
                ParticipantId = pri.ParticipantId,
                Approved = pri.AcceptedOn!.Value,
                SupportWorker = u!.Id,
                CommunityLocationId = l.Id,
                ContractId = l.Contract!.Id,
                LocationType = l.LocationType.Name,
                CustodyLocationId = pri.CustodyLocationId,
                MeetingAttendedOn = pri.MeetingAttendedOn,
                TenantId = u!.TenantId!,
                DateOfFirstConsent = p.DateOfFirstConsent,
                CountOfPayments = (
                    from sp in db.SupportAndReferralPayments
                    where sp.ParticipantId == p.Id
                          && sp.EligibleForPayment
                          && sp.SupportType == "Through the Gate"
                    select sp.Id
                ).Count(),
                Tasks = (
                    db.PathwayPlans
                        .SelectMany(pp => pp.Objectives)
                        .SelectMany(o => o.Tasks)
                        .Where(t => t.ObjectiveId == pri.ObjectiveId && t.IsMandatory)
                        .Select(t => new TaskData {
                            IsMandatory = t.IsMandatory,
                            CompletionStatus = t.CompletedStatus!,
                            Completed = t.Completed,
                            Index = t.Index
                        })
                ).ToArray(),
                ActualReleaseDate = pri.ActualReleaseDate!.Value,
                Locations = (
                    db.ParticipantLocationHistories
                        .Where(h => h.ParticipantId == p.Id)
                        .Select(h => new LocationData {
                                LocationId = h.LocationId,
                                From = h.From
                            }
                        ).ToArray()
                )
            };
        return await query.FirstAsync();
    }

    /// <summary>
    /// This record represents the data required to calculate the details
    /// of a pre-release payment of 
    /// </summary>
    public record Data
    {
        public required Guid PriId { get; init; }
        public required DateTime ActivityInput { get; init; }
        public required string ParticipantId { get; init; }
        public required DateTime Approved { get; init; }
        public required string SupportWorker { get; init; }
        public required int CustodyLocationId { get; init; }
        public required int CommunityLocationId { get; init; }
        public required string LocationType { get; init; }
        public required string ContractId { get; init; }
        public required DateOnly MeetingAttendedOn { get; init; }
        public required string TenantId { get; init; }
        public required DateOnly? DateOfFirstConsent { get; init; }
        public required int? CountOfPayments { get; init; }
        public DateOnly PaymentPeriod
        {
            get
            {
                List<DateOnly> dates =
                [
                    DateOnly.FromDateTime(Approved),
                    new(DateTime.Now.Year, DateTime.Now.Month, 1)
                ];

                if (DateOfFirstConsent is not null)
                {
                    dates.Add(DateOfFirstConsent.Value);
                }
                return dates.Max();
            }
        }

        public bool MeetingTookPlaceOnOrAfterConsent => MeetingAttendedOn >= DateOfFirstConsent;
        public DateOnly ActualReleaseDate { get; init; }

        public required TaskData[] Tasks { get; init; }
        public required LocationData[] Locations { get; init; }

        public bool AllTasksCompleted
            => Tasks.All(t => t.CompletionStatus == CompletionStatus.Done);

        public bool TaskTwoCompletedWithin4Weeks
        {
            get
            {
                var task2 = Tasks.First(t => t.Index == 2);
                if (task2.CompletionStatus != CompletionStatus.Done)
                {
                    return false;
                }

                DateTime maximumReleaseDate = ActualReleaseDate.ToDateTime(TimeOnly.MinValue).AddDays(28);
                DateTime completionDate = task2.Completed!.Value.Date;

                return completionDate <= maximumReleaseDate;

            }
        }

        public bool ParticipantInCommunityLocationAfterCustodyLocation
        {
            get
            {
                // we are only interested in the 90 days before the actual release date

                bool beenInCustody = false;
                bool beenInCommunityAfter = false;


                foreach (var h in Locations.Where(h => h.From < ActualReleaseDate!.ToDateTime(TimeOnly.MinValue)
                                 .AddDays(14)) // add 14 days to allow for DMS sync issues
                             .OrderBy(h => h.From))
                {
                    if (h.LocationId == CustodyLocationId)
                    {
                        beenInCustody = true;
                        continue;
                    }

                    if (h.LocationId == CommunityLocationId && beenInCustody)
                    {
                        beenInCommunityAfter = true;
                        break;
                    }
                }

                return beenInCustody && beenInCommunityAfter;

            }
        }
    }

    /// <summary>
    /// This record represents task data returned from the query
    /// </summary>
    public record TaskData
    {
        public required bool IsMandatory { get; init; }
        public required CompletionStatus CompletionStatus { get; init; } 
        public required DateTime? Completed { get; init; }
        public required int Index { get; init; }
    }

    public record LocationData
    {
        public required int LocationId { get; init; }
        public required DateTime From { get; init; }
    }
}