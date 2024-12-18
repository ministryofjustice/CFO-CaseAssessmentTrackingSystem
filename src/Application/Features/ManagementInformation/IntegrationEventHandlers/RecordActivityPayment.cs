using Cfo.Cats.Application.Features.Payables.IntegrationEvents;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Domain.Entities.Payables;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordActivityPayment(IManagementInformationDbContext miContext, IApplicationDbContext applicationDbContext) : IConsumer<ActivityApprovedIntegrationEvent>
{
    private static class IneligibilityReasons
    {
        public const string AlreadyPaidThisMonth = "This activity has already been paid to this contract, for this participant, this month.";
    }

    public async Task Consume(ConsumeContext<ActivityApprovedIntegrationEvent> context)
    {
        var activity = await applicationDbContext.Activities
            .Include(a => a.TookPlaceAtContract)
            .Include(a => a.TookPlaceAtLocation)
            .AsNoTracking()
            .SingleAsync(activity => activity.Id == context.Message.Id);

        // do we already have a payment for:
        // this activity
        // for this month
        // for this contract
        // for this participant
        var previousPayments = await miContext.ActivityPayments
            .Where(p => p.ActivityCategory == activity.Category // this is now very much dependant on the paymech rules. do we go off the definition? category? activity type? a combination?
                   && p.ActivityApproved.HappenedThisMonth()
                   && p.ContractId == activity.TookPlaceAtContract.Id 
                   && p.ParticipantId == activity.ParticipantId)
            .ToListAsync() ?? [];

        string? ineligibilityReason = null;

        if(previousPayments.Count is > 0)
        {
            // Bespoke activity type logic
            if(activity is EducationTrainingActivity educationTrainingActivity)
            {
                var previouslyPaidEducationAndTrainingActivityIds = previousPayments
                    .Where(payment => payment.ActivityType == educationTrainingActivity.Type)
                    .Select(payment => payment.ActivityId);

                bool isDuplicate = await applicationDbContext.EducationTrainingActivities
                    .Where(eta => previouslyPaidEducationAndTrainingActivityIds.Contains(eta.Id))
                    .Where(eta => eta.CourseLevel == educationTrainingActivity.CourseLevel && eta.CourseTitle == educationTrainingActivity.CourseTitle)
                    .AnyAsync();

                ineligibilityReason = isDuplicate ? IneligibilityReasons.AlreadyPaidThisMonth : null;
            }
            else
            {
                ineligibilityReason = IneligibilityReasons.AlreadyPaidThisMonth;
            }

        }

        var payment = new ActivityPaymentBuilder()
            .WithActivity(activity.Id)
            .WithActivityCategory(activity.Category)
            .WithActivityType(activity.Type)
            .WithParticipantId(activity.ParticipantId)
            .WithContractId(activity.TookPlaceAtContract.Id)
            .WithApproved(activity.ApprovedOn!.Value)
            .WithLocationId(activity.TookPlaceAtLocation.Id)
            .WithLocationType(activity.TookPlaceAtLocation.LocationType.Name)
            .WithTenantId(activity.TenantId)
            .WithEligibleForPayment(ineligibilityReason is null)
            .WithIneligibilityReason(ineligibilityReason)
            .Build();

        miContext.ActivityPayments.Add(payment);
        await miContext.SaveChangesAsync(CancellationToken.None);
    }

}

static class Ext
{
    internal static bool HappenedThisMonth(this DateTime datetime) =>
        DateTime.UtcNow.Month == datetime.Month
        && DateTime.UtcNow.Year == datetime.Year;
}