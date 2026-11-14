using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordWingInductionPaymentConsumer(IUnitOfWork unitOfWork) : IHandleMessages<WingInductionCreatedIntegrationEvent>
{

    public async Task Handle(WingInductionCreatedIntegrationEvent context)
    {
        var induction = await unitOfWork.DbContext.WingInductions
            .Include(a => a.Location)
                .ThenInclude(l => l!.Contract)
            .Include(a => a.Owner)
            .AsNoTracking()
            .SingleAsync(i => i.Id == context.WingInductionId);

        IneligibilityReason? ineligibilityReason = null;
       
        var enrolmentHasBeenApproved = await unitOfWork.DbContext.ParticipantEnrolmentHistories
            .AsNoTracking()
            .Where(e => e.ParticipantId == induction.ParticipantId &&
                        e.EnrolmentStatus == EnrolmentStatus.ApprovedStatus.Value)
            .AnyAsync();

        if(enrolmentHasBeenApproved == false)
        {
            ineligibilityReason = IneligibilityReason.MaximumPaymentLimitReached;
        }

        if (enrolmentHasBeenApproved == false)
        {
            ineligibilityReason = IneligibilityReason.NotYetApproved;
        }

        if(ineligibilityReason is null)
        {
            var hasPreviousPaymentForContract = await unitOfWork.DbContext
             .InductionPayments
             .AnyAsync(c => c.ParticipantId == induction.ParticipantId
                         && c.ContractId == induction.Location!.Contract!.Id
                         && c.LocationType == LocationType.Wing.Name
                         && c.EligibleForPayment);
            
            if(hasPreviousPaymentForContract)
            {
                ineligibilityReason = IneligibilityReason.MaximumPaymentLimitReached;    
            }
        }

         InductionPayment payment = ineligibilityReason switch
        {
            not null => InductionPayment.CreateNonPayableInductionPayment(induction, ineligibilityReason),
            _ => InductionPayment.CreatePayableInductionPayment(induction)
        };

        unitOfWork.DbContext.InductionPayments.Add(payment);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);

    }
}