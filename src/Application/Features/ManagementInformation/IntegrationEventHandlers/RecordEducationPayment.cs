using Cfo.Cats.Application.Features.Activities.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordEducationPayment(IUnitOfWork unitOfWork)
    : IConsumer<ActivityApprovedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ActivityApprovedIntegrationEvent> context)
    {
        var activity = await unitOfWork.DbContext.EducationTrainingActivities
            .Include(a => a.TookPlaceAtContract)
            .Include(a => a.TookPlaceAtLocation)
            .AsNoTracking()
            .SingleOrDefaultAsync(activity => activity.Id == context.Message.Id);

        if (activity is null)
        {
            // we are only interested in Education
            return;
        }

        IneligibilityReason? ineligibilityReason = null;
        
        var history = unitOfWork.DbContext.ParticipantEnrolmentHistories
            .AsNoTracking()
            .Where(e => e.ParticipantId == activity.ParticipantId &&
                        e.EnrolmentStatus == EnrolmentStatus.ApprovedStatus)
            .MinBy(e => e.Created);

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

            var previousPaymentsQuery = from ap in unitOfWork.DbContext.EducationPayments
                where
                    ap.ParticipantId == activity.ParticipantId
                    && ap.ContractId == activity.TookPlaceAtContract.Id
                    && ap.CourseLevel == activity.CourseLevel
                    && ap.CourseTitle == activity.CourseTitle
                    && ap.EligibleForPayment
                select ap;

            var previousPayments = await previousPaymentsQuery.AsNoTracking().ToListAsync();


            if (previousPayments.Count > 0)
            {
                ineligibilityReason = IneligibilityReason.MaximumPaymentLimitReached;
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

        }

        var payment = ineligibilityReason switch
        {
            not null => EducationPayment.CreateNonPayableEducationPayment(activity, ineligibilityReason),
            _ => EducationPayment.CreatePayableEducationPayment(activity, history!.Created!.Value)
        };

        unitOfWork.DbContext.EducationPayments.Add(payment);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }


}