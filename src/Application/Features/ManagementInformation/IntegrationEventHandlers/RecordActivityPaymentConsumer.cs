using Cfo.Cats.Domain.Entities.ManagementInformation;
using Cfo.Cats.Domain.Entities.Activities;
using MassTransit;
using Cfo.Cats.Application.Features.Activities.IntegrationEvents;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordActivityPaymentConsumer(IUnitOfWork unitOfWork) : IConsumer<ActivityApprovedIntegrationEvent>
{
    private static class IneligibilityReasons
    {
        public const string AlreadyPaidThisMonth = "An activity of this type has already been paid to this contract, for this participant, this month.";
    }

    public async Task Consume(ConsumeContext<ActivityApprovedIntegrationEvent> context)
    {

        var activity = await unitOfWork.DbContext.Activities
            .Include(a => a.TookPlaceAtContract)
            .Include(a => a.TookPlaceAtLocation)
            .AsNoTracking()
            .SingleAsync(activity => activity.Id == context.Message.Id);


        if (activity.Type == ActivityType.Employment || activity.Type == ActivityType.EducationAndTraining)
        {
            // we do not record ETE events here.
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
        
        var query = from ap in unitOfWork.DbContext.ActivityPayments
            where
                  ap.ParticipantId == activity.ParticipantId
                  && ap.ContractId == activity.TookPlaceAtContract.Id
                  && ap.ActivityCategory == activity.Category.Name
                  && ap.ActivityType == activity.Type.Name
                  && ap.ActivityApproved >= dates.TheFirstOfMonth
                  && ap.ActivityApproved <= dates.TheLastOfMonth
            select ap;

        var previousPayments = await query.AsNoTracking().ToListAsync();
                                 
        string? ineligibilityReason = null;

        if(previousPayments.Count > 0)
        {
            ineligibilityReason = IneligibilityReasons.AlreadyPaidThisMonth;
        }

        var payment = new ActivityPaymentBuilder()
            .WithActivity(activity.Id)
            .WithActivityCategory(activity.Category.Name)
            .WithActivityType(activity.Type.Name)
            .WithParticipantId(activity.ParticipantId)
            .WithContractId(activity.TookPlaceAtContract.Id)
            .WithApproved(activity.ApprovedOn!.Value)
            .WithLocationId(activity.TookPlaceAtLocation.Id)
            .WithLocationType(activity.TookPlaceAtLocation.LocationType.Name)
            .WithTenantId(activity.TenantId)
            .WithEligibleForPayment(ineligibilityReason is null)
            .WithIneligibilityReason(ineligibilityReason)
            .Build();

        unitOfWork.DbContext.ActivityPayments.Add(payment);
        await unitOfWork.SaveChangesAsync(CancellationToken.None);
    }

}
