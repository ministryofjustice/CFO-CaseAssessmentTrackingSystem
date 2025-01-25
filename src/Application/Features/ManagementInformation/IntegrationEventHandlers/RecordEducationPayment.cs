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

        var dates = await unitOfWork.DbContext.DateDimensions
            .Where(dd => dd.TheDate == activity.ApprovedOn!.Value)
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
                && ap.ActivityApproved >= dates.TheFirstOfMonth
                && ap.ActivityApproved <= dates.TheLastOfMonth
                && ap.EligibleForPayment
            select ap;

        var previousPaymentIds = await previousPaymentsQuery
                .Select(ap => new { ap.ActivityId })
                .AsNoTracking()
                .ToListAsync();

        string? ineligibilityReason = null;

        if (previousPaymentIds.Any())
        {
            var previousSameEducationActivities = await unitOfWork.DbContext.EducationTrainingActivities
                    .Where(a => previousPaymentIds.Select(p => p.ActivityId).Contains(a.Id)
                                && a.CourseTitle == activity.CourseTitle
                                && a.CourseLevel == activity.CourseLevel)
                    .AsNoTracking()
                    .ToListAsync();

            if (previousSameEducationActivities.Any())
            {
                ineligibilityReason = IneligibilityReasons.AlreadyPaidThisMonth;
            }
        }

        var history = await unitOfWork.DbContext.ParticipantEnrolmentHistories
            .Where(h => h.ParticipantId == activity.ParticipantId)
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
                .Where(p => p.Id == activity.ParticipantId)
                .Select(p => p.DateOfFirstConsent)
                .FirstAsync();

            if (consentDate!.Value > DateOnly.FromDateTime(activity.CommencedOn))
            {
                ineligibilityReason = IneligibilityReasons.BeforeConsent;
            }
        }

        var payment = new EducationPaymentBuilder()
            .WithActivity(activity.Id)
            .WithParticipantId(activity.ParticipantId)
            .WithContractId(activity.TookPlaceAtContract.Id)
            .WithApproved(activity.ApprovedOn!.Value)
            .WithLocationId(activity.TookPlaceAtLocation.Id)
            .WithLocationType(activity.TookPlaceAtLocation.LocationType.Name)
            .WithTenantId(activity.TenantId)
            .WithEligibleForPayment(ineligibilityReason is null)
            .WithIneligibilityReason(ineligibilityReason)
            .WithCourseTitle(activity.CourseTitle)
            .WithCourseLevel(activity.CourseLevel)
            .Build();

        unitOfWork.DbContext.EducationPayments.Add(payment);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

    private static class IneligibilityReasons
    {
        public const string AlreadyPaidThisMonth = "This education activity has already been paid to this contract, for this participant, this month.";
        public const string NotYetApproved = "The enrolment for this participant has not yet been approved";
        public const string BeforeConsent = "This occurred before the consent date";
    }
}