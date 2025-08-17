using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordPreReleaseSupportPayment(IUnitOfWork unitOfWork) : IHandleMessages<PRIAssignedIntegrationEvent>
{
    public async Task Handle(PRIAssignedIntegrationEvent context)
    {
        var data = await GetData(context);

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
            _ => null,
        };

        var payment = ineligibilityReason switch
        {
            null => CreatePayable(data),
            _ => CreateNonPayable(data, ineligibilityReason)
        };

        return payment;
    }

    private static SupportAndReferralPayment CreatePayable(Data data)
        => SupportAndReferralPayment.CreatePayable(
            priId: data.PriId,
            participantId: data.ParticipantId,
            supportType: "Pre-Release Support",
            activityInput: DateOnly.FromDateTime(data.ActivityInput),
            approvedDate: DateOnly.FromDateTime(data.Approved),
            supportWorker: data.SupportWorker,
            contractId: data.ContractId,
            locationType: data.LocationType,
            locationId: data.LocationId,
            tenantId: data.TenantId,
            paymentPeriod: data.PaymentPeriod
        );

    private static SupportAndReferralPayment CreateNonPayable(Data data, IneligibilityReason reason)
        => SupportAndReferralPayment.CreateNonPayable(
            priId: data.PriId,
            participantId: data.ParticipantId,
            supportType: "Pre-Release Support",
            activityInput: DateOnly.FromDateTime(data.ActivityInput),
            approvedDate: DateOnly.FromDateTime(data.Approved),
            supportWorker: data.SupportWorker,
            contractId: data.ContractId,
            locationType: data.LocationType,
            locationId: data.LocationId,
            tenantId: data.TenantId,
            paymentPeriod: data.PaymentPeriod,
            reason: reason
        );

    private async Task<Data> GetData(PRIAssignedIntegrationEvent context)
    {
        var db = unitOfWork.DbContext;

        var query = from p in db.Participants
            join pri in db.PRIs on p.Id equals pri.ParticipantId
            join l in db.Locations on pri.CustodyLocationId equals l.Id
            join u in db.Users on pri.CreatedBy equals u.Id
            where pri.Id == context.PRIId
            select new Data
            {
                PriId = pri.Id,
                ActivityInput = pri.Created!.Value,
                ParticipantId = pri.ParticipantId,
                Approved = pri.AcceptedOn!.Value,
                SupportWorker = u!.Id,
                LocationId = l.Id,
                LocationType = l.LocationType.Name,
                ContractId = l.Contract!.Id,
                MeetingAttendedOn = pri.MeetingAttendedOn,
                TenantId = u!.TenantId!,
                DateOfFirstConsent = p.DateOfFirstConsent,
                CountOfPayments = (
                    from sp in db.SupportAndReferralPayments
                    where sp.ParticipantId == p.Id
                          && sp.EligibleForPayment
                          && sp.SupportType == "Pre-Release Support"
                    select 1
                ).Count()
            };
        return await query.FirstAsync();
    }

    /// <summary>
    /// This class represents the data required to calculate the details
    /// of a pre-release payment of 
    /// </summary>
    public record Data
    {
        public required Guid PriId { get; init; }
        public required DateTime ActivityInput { get; init; }
        public required string ParticipantId { get; init; }
        public required DateTime Approved { get; init; }
        public required string SupportWorker { get; init; }

        public required int LocationId { get; init; }
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
    }

}