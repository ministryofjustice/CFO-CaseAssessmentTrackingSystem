using Cfo.Cats.Application.Features.PRIs.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordPreReleaseSupportPayment(IUnitOfWork unitOfWork) : IConsumer<PRIAssignedIntegrationEvent>
{
    private static class IneligibilityReasons
    {
        public const string AlreadyPaid = "A Pre-Release Support payment has already been claimed for this participant";
        public const string NotYetApproved = "The enrolment for this participant has not yet been approved";
        public const string BeforeConsent = "This occurred before the consent date";
    }

    private const string SupportType = "Pre-Release Support";

    public async Task Consume(ConsumeContext<PRIAssignedIntegrationEvent> context)
    {

        var query = from p in unitOfWork.DbContext.PRIs
            join l in unitOfWork.DbContext.Locations on p.CustodyLocationId equals l.Id
            join u in unitOfWork.DbContext.Users on p.CreatedBy equals u.Id
            where p.Id == context.Message.PRIId
            select new
            {
                p.Id,
                p.ParticipantId,
                p.CustodyLocationId,
                l.LocationType,
                ContractId = l.Contract!.Id,
                u.TenantId,
                p.CreatedBy,
                p.MeetingAttendedOn
            };

        var pri = await query.FirstAsync();

        string? ineligibilityReason = null;

        // to be eligible for payment your enrolment must have been approved.
        // and we must not have had a payment for the same type of induction
        if (await unitOfWork.DbContext
                .SupportAndReferralPayments
                .AnyAsync(c => c.ParticipantId == pri.ParticipantId
                               && c.SupportType == SupportType
                               && c.EligibleForPayment))
        {
            ineligibilityReason = IneligibilityReasons.AlreadyPaid;
        }

        var history = await unitOfWork.DbContext.ParticipantEnrolmentHistories
            .AsNoTracking()
            .Where(h => h.ParticipantId == pri.ParticipantId)
            .ToListAsync();

        var firstApproval = history.Where(h => h.EnrolmentStatus == EnrolmentStatus.ApprovedStatus)
            .Min(x => x.Created);

        if (firstApproval.HasValue == false)
        {
            ineligibilityReason = IneligibilityReasons.NotYetApproved;
        }

        if (ineligibilityReason is null)
        {
            var consentDate = await unitOfWork.DbContext
                .Participants
                .AsNoTracking()
                .Where(p => p.Id == pri.ParticipantId)
                .Select(p => p.DateOfFirstConsent)
                .FirstAsync();

            if (consentDate!.Value > pri.MeetingAttendedOn)
            {
                ineligibilityReason = IneligibilityReasons.BeforeConsent;
            }
        }

        var payment = new SupportAndReferralBuilder()
            .WithParticipantId(pri.ParticipantId)
            .WithPri(pri.Id)
            .WithApproved(context.Message.OccurredOn.Date)
            .WithLocationId(pri.CustodyLocationId)
            .WithContractId(pri.ContractId)
            .WithSupportType(SupportType)
            .WithTenantId(pri.TenantId)
            .WithLocationType(pri.LocationType.Name)
            .WithEligibleForPayment(ineligibilityReason == null)
            .WithIneligibilityReason(ineligibilityReason)
            .WithSupportWorker(pri.CreatedBy)
            .Build();

        unitOfWork.DbContext.SupportAndReferralPayments.Add(payment);

        await unitOfWork.SaveChangesAsync();
            

        await Task.CompletedTask;

    }
}