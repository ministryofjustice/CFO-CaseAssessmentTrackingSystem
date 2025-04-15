using Cfo.Cats.Application.Features.Assessments.IntegrationEvents;
using Cfo.Cats.Domain.Entities.Assessments;
using Cfo.Cats.Domain.Entities.ManagementInformation;
using MassTransit;

namespace Cfo.Cats.Application.Features.ManagementInformation.IntegrationEventHandlers;

public class RecordReassessmentPaymentConsumer(IUnitOfWork unitOfWork) : IConsumer<AssessmentScoredIntegrationEvent>
{
    public async Task Consume(ConsumeContext<AssessmentScoredIntegrationEvent> context)
    {
        if(await unitOfWork.DbContext.ParticipantAssessments.CountAsync(a => a.ParticipantId == context.Message.ParticipantId) is 1)
        {
            // We are only interested in reassessments
            return;
        }

        var data = await GetData(context);

        var ineligibilityReason = data switch
        {
            { DateOfFirstConsent: null } => IneligibilityReason.NotYetApproved,
            { NumberOfPaymentsInLastTwoPaymentMonths: > 0 } => IneligibilityReason.MaximumPaymentLimitReached,
            { WasInitialAssessmentCompletedInLastTwoPaymentMonths: true } => IneligibilityReason.InitialAssessmentCompletedInLastTwoMonths,
            _ => null,
        };

        var payment = ineligibilityReason switch
        {
            null => CreatePayable(data),
            _ => CreateNonPayable(data, ineligibilityReason)
        };

        unitOfWork.DbContext.ReassessmentPayments.Add(payment);
        await unitOfWork.SaveChangesAsync();
    }

    static ReassessmentPayment CreatePayable(Data data)
        => ReassessmentPayment.CreatePayable(data.Assessment, data.ContractId, data.LocationId, data.LocationType);

    static ReassessmentPayment CreateNonPayable(Data data, IneligibilityReason ineligibilityReason)
        => ReassessmentPayment.CreateNonPayable(data.Assessment, data.ContractId, data.LocationId, data.LocationType, ineligibilityReason);
    
    async Task<Data> GetData(ConsumeContext<AssessmentScoredIntegrationEvent> context)
    {
        var db = unitOfWork.DbContext;

        var twoMonthsAgo = context.Message.OccurredOn.Date.AddMonths(-2);
        var twoPaymentMonthsAgo = new DateTime(twoMonthsAgo.Year, twoMonthsAgo.Month, day: 1);

        var query = from p in db.Participants
                    join a in db.ParticipantAssessments on p.Id equals a.ParticipantId
                    join l in db.Locations on a.LocationId equals l.Id
                    where a.Id == context.Message.Id
                    select new Data
                    {
                        Assessment = a,
                        NumberOfPaymentsInLastTwoPaymentMonths = (
                            from rp in db.ReassessmentPayments
                            join ddTo in db.DateDimensions on context.Message.OccurredOn.Date equals ddTo.TheDate
                            where rp.ParticipantId == p.Id
                                  && rp.EligibleForPayment
                                  && rp.AssessmentCompleted.Date > twoPaymentMonthsAgo
                                  && rp.AssessmentCompleted.Date < ddTo.TheLastOfMonth
                            select 1
                        ).Count(),
                        ContractId = l.Contract!.Id,
                        LocationId = l.Id,
                        LocationType = l.LocationType.Name,
                        DateOfFirstConsent = p.DateOfFirstConsent,
                        WasInitialAssessmentCompletedInLastTwoPaymentMonths = (
                            from a in db.ParticipantAssessments
                            where a.ParticipantId == p.Id
                            orderby a.Created ascending
                            select a
                        ).First().Completed!.Value.Date >= twoPaymentMonthsAgo
                    };

        return await query.FirstAsync();
    }


    record Data
    {
        public required ParticipantAssessment Assessment { get; set; }
        public required int? NumberOfPaymentsInLastTwoPaymentMonths { get; init; }
        public required bool WasInitialAssessmentCompletedInLastTwoPaymentMonths { get; init; }
        public required string ContractId { get; set; }
        public required int LocationId { get; set; }
        public required string LocationType { get; set; }
        public required DateOnly? DateOfFirstConsent { get; init; }

    }
}
