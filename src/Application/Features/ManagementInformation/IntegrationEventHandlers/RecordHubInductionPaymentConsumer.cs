using Cfo.Cats.Application.Features.Inductions.IntegrationEvents;
using Cfo.Cats.Domain.Common.Enums;
using Cfo.Cats.Domain.Entities.Inductions;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordHubInductionPaymentConsumer(IUnitOfWork unitOfWork) : IHandleMessages<HubInductionCreatedIntegrationEvent>
{

    public async Task Handle(HubInductionCreatedIntegrationEvent context)
    {
        var inductionData = await unitOfWork.DbContext.HubInductions
            .Include(a => a.Location)
                .ThenInclude(l => l!.Contract)
            .Include(a => a.Owner)
            .AsNoTracking()
            .SingleAsync(i => i.Id == context.Id);

        IneligibilityReason? ineligibilityReason = null;

        // to be eligible for payment your enrolment must have been approved.
        // and we must not have had a payment for the same type of induction
        if (await unitOfWork.DbContext
            .InductionPayments
            .AnyAsync(c => c.ParticipantId == inductionData.ParticipantId
                        && c.ContractId == inductionData.Location!.Contract!.Id
                        && c.LocationType == "Hub" 
                        &&  c.EligibleForPayment) )
        {
            ineligibilityReason = IneligibilityReason.MaximumPaymentLimitReached;
        }

        var history = await unitOfWork.DbContext.ParticipantEnrolmentHistories
            .AsNoTracking()
            .Where(e => e.ParticipantId == inductionData.ParticipantId &&
                        e.EnrolmentStatus == EnrolmentStatus.ApprovedStatus.Value)
            .OrderBy(e => e.Created)
            .FirstOrDefaultAsync();

        if (history == null)        
        {
            ineligibilityReason = IneligibilityReason.NotYetApproved;
        }

        if (ineligibilityReason is null)
        {
            var dates = await unitOfWork.DbContext.DateDimensions
                .Where(dd => dd.TheDate == inductionData.InductionDate)
                .Select(dd => new
                {
                    dd.TheFirstOfMonth,
                    dd.TheLastOfMonth
                })
                .SingleAsync();

            var query = from hubip in unitOfWork.DbContext.InductionPayments
                        where
                            hubip.ParticipantId == inductionData.ParticipantId
                            && hubip.ContractId == inductionData.Location!.Contract!.Id
                            && hubip.LocationId == inductionData.LocationId
                            && hubip.LocationType == inductionData.Location!.LocationType.Name
                            && hubip.CommencedDate >= dates.TheFirstOfMonth
                            && hubip.CommencedDate <= dates.TheLastOfMonth
                            && hubip.EligibleForPayment
                        select hubip;

            var previousPayments = await query.AsNoTracking().ToListAsync();


            if (previousPayments.Count > 0)
            {
                ineligibilityReason = IneligibilityReason.MaximumPaymentLimitReached;
            }
        }

        if (ineligibilityReason is null)
        {
            var consentDate = await unitOfWork.DbContext
                .Participants
                .AsNoTracking()
                .Where(p => p.Id == inductionData.ParticipantId)
                .Select(p => p.DateOfFirstConsent)
                .FirstAsync();

            if (consentDate!.Value > DateOnly.FromDateTime(inductionData.InductionDate))
            {
                ineligibilityReason = IneligibilityReason.BeforeConsent;
            }
        }

        InductionPayment payment = ineligibilityReason switch
        {
            not null => InductionPayment.CreateNonPayableInductionPayment(inductionData, ineligibilityReason),
            _ => InductionPayment.CreatePayableInductionPayment(inductionData, history!.Created!.Value)
        };


        unitOfWork.DbContext.InductionPayments.Add(payment);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);

    }
}