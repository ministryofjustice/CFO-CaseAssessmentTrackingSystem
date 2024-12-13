using System.Net;
using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Inductions;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordWingInductionPayment(IApplicationDbContext applicationDb, IManagementInformationDbContext miDb) : IConsumer<WingInductionCreatedIntegrationEvent>
{

    private static class IneligibilityReasons
    {
        public const string AlreadyPaid = "A wing induction has already been claimed under this contract";
        public const string NotYetApproved = "The enrolment for this participant has not yet been approved";
    }

    public async Task Consume(ConsumeContext<WingInductionCreatedIntegrationEvent> context)
    {
        var inductionData = await applicationDb
            .WingInductions
            .Where(i => i.Id == context.Message.Id)
            .Select(x => new
            {
                x.ParticipantId,
                x.CreatedBy,
                x.LocationId,
                ContractId = x.Location!.Contract!.Id,
                x.InductionDate,
                LocationType = x.Location.LocationType.Name,
                TenantId = x.Owner!.TenantId!
            })
            .SingleAsync();

        string? ineligibilityReason = null;

        // to be eligible for payment your enrolment must have been approved.
        // and we must not have had a payment for the same type of induction
        if (await miDb.InductionPayments.AnyAsync(c => c.ParticipantId == inductionData.ParticipantId
            && c.ContractId == inductionData.ContractId && c.LocationType == inductionData.LocationType && c.EligibleForPayment))
        {
            ineligibilityReason = IneligibilityReasons.AlreadyPaid;
        }

        var history = await applicationDb.ParticipantEnrolmentHistories
                                .Where(h => h.ParticipantId == inductionData.ParticipantId)
                                .ToListAsync();

        var firstApproval = history.Where(h => h.EnrolmentStatus == EnrolmentStatus.ApprovedStatus)
                            .Min(x => x.Created);

        if (firstApproval.HasValue == false)
        {
            ineligibilityReason = IneligibilityReasons.NotYetApproved;
        }

        var payment = new InductionPaymentBuilder()
                    .WithParticipantId(inductionData.ParticipantId)
                    .WithSupportWorker(inductionData.CreatedBy!)
                    .WithContractId(inductionData.ContractId)
                    .WithApproved(firstApproval)
                    .WithInduction(inductionData.InductionDate.Date)
                    .WithLocationId(inductionData.LocationId)
                    .WithLocationType(inductionData.LocationType)
                    .WithTenantId(inductionData.TenantId)
                    .WithEligibleForPayment(ineligibilityReason == null)
                    .WithIneligibilityReason(ineligibilityReason)
                    .Build();

        miDb.InductionPayments.Add(payment);
        await miDb.SaveChangesAsync(CancellationToken.None);

    }
}