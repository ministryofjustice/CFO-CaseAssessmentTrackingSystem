using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Rebus.Handlers;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordActivityPaymentConsumer(IUnitOfWork unitOfWork) : IHandleMessages<ActivityApprovedIntegrationEvent>
{
    public async Task Handle(ActivityApprovedIntegrationEvent context)
    {
        var activity = await unitOfWork.DbContext.Activities
            .Include(a => a.TookPlaceAtContract)
            .Include(a => a.TookPlaceAtLocation)
            .AsNoTracking()
            .SingleAsync(activity => activity.Id == context.Id);

        if (activity.Type == ActivityType.Employment || activity.Type == ActivityType.EducationAndTraining)
        {
            // we do not record ETE events here.
            return;
        }

        if (activity.CompletedOn is null)
        {
            // we do not record "payment" events if the activity is not approved
            return;
        }

        if (activity.Status==ActivityStatus.AbandonedStatus)
        {
            // we do not record "payment" events if the activity has been abandoned
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

            var query = from ap in unitOfWork.DbContext.ActivityPayments
                where
                    ap.ParticipantId == activity.ParticipantId
                    && ap.ContractId == activity.TookPlaceAtContract.Id
                    && ap.ActivityCategory == activity.Category.Name
                    && ap.ActivityType == activity.Type.Name
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
            // if we get to here, we are approved and should have a consent date on the participant
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

        ActivityPayment payment = ineligibilityReason switch
        {
            not null => ActivityPayment.CreateNonPayableActivityPayment(activity, ineligibilityReason),
            _ => ActivityPayment.CreatePayableActivityPayment(activity, history!.Created!.Value)
        };

        unitOfWork.DbContext.ActivityPayments.Add(payment);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }
}