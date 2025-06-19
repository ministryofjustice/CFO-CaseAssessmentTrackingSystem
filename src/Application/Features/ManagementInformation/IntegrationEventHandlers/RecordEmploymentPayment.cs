using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordEmploymentPayment(IUnitOfWork unitOfWork)
    : IHandleMessages<ActivityApprovedIntegrationEvent>
{
    public async Task Handle(ActivityApprovedIntegrationEvent context)
    {
        var activity = await unitOfWork.DbContext.EmploymentActivities
            .Include(a => a.TookPlaceAtContract)
            .Include(a => a.TookPlaceAtLocation)
            .AsNoTracking()
            .SingleOrDefaultAsync(activity => activity.Id == context.Id);

        if (activity is null)
        {
            // we are only interested in Employment
            return;
        }

        IneligibilityReason? ineligibilityReason = null;

        var history = await unitOfWork.DbContext.ParticipantEnrolmentHistories
            .AsNoTracking()
            .Where(e => e.ParticipantId == activity.ParticipantId &&
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
                .Where(dd => dd.TheDate == activity.CommencedOn.Date)
                .Select(dd => new
                {
                    dd.TheFirstOfMonth,
                    dd.TheLastOfMonth
                })
                .SingleAsync();

            var query = from ap in unitOfWork.DbContext.EmploymentPayments
                where
                    ap.ParticipantId == activity.ParticipantId
                    && ap.ContractId == activity.TookPlaceAtContract.Id
                    && ap.CommencedDate >= dates.TheFirstOfMonth
                    && ap.CommencedDate <= dates.TheLastOfMonth
                    && ap.EligibleForPayment
                select ap;

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
                .Where(p => p.Id == activity.ParticipantId)
                .Select(p => p.DateOfFirstConsent)
                .FirstAsync();

            if (consentDate!.Value > DateOnly.FromDateTime(activity.CommencedOn))
            {
                ineligibilityReason = IneligibilityReason.BeforeConsent;
            }
        }

        EmploymentPayment payment = ineligibilityReason switch
        {
            not null => EmploymentPayment.CreateNonPayableEmploymentPayment(activity, ineligibilityReason),
            _ => EmploymentPayment.CreateEmploymentPayment(activity, history!.Created!.Value)
        };

        unitOfWork.DbContext.EmploymentPayments.Add(payment);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);

    }
    
}