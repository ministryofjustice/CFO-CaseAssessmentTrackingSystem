using Cfo.Cats.Application.Features.Assessments.IntegrationEvents;
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

        var payment = GeneratePayment(data);

        unitOfWork.DbContext.ReassessmentPayments.Add(payment);
        await unitOfWork.SaveChangesAsync();
    }

    public static ReassessmentPayment GeneratePayment(Data data)
    {
        var ineligibilityReason = data switch
        {
            { DateOfFirstConsent: null } => IneligibilityReason.NotYetApproved,
            { HaveAnyPaymentsBeenMadeInLastTwoPaymentMonths: true } => IneligibilityReason.MaximumPaymentLimitReached,
            { WasInitialAssessmentCompletedInLastTwoPaymentMonths: true } => IneligibilityReason.InitialAssessmentCompletedInLastTwoMonths,
            _ => null,
        };

        var payment = ineligibilityReason switch
        {
            null => CreatePayable(data),
            _ => CreateNonPayable(data, ineligibilityReason)
        };

        return payment;
    }

    static ReassessmentPayment CreatePayable(Data data)
        => ReassessmentPayment.CreatePayable(data.AssessmentId, data.Completed, data.Created, data.ParticipantId, data.TenantId, data.SupportWorker, data.ContractId, data.LocationId, data.LocationType);

    static ReassessmentPayment CreateNonPayable(Data data, IneligibilityReason ineligibilityReason)
        => ReassessmentPayment.CreateNonPayable(data.AssessmentId, data.Completed, data.Created, data.ParticipantId, data.TenantId, data.SupportWorker, data.ContractId, data.LocationId, data.LocationType, ineligibilityReason);
    
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
                        PreviouslyPaidAssessments = (
                            from rp in db.ReassessmentPayments
                            join ddTo in db.DateDimensions on context.Message.OccurredOn.Date equals ddTo.TheDate
                            where rp.ParticipantId == p.Id
                                  && rp.EligibleForPayment
                                  && rp.PaymentPeriod.Date <= ddTo.TheLastOfMonth
                            select new Data.Assessment(rp.PaymentPeriod)
                        ),
                        ContractId = l.Contract!.Id,
                        LocationId = l.Id,
                        LocationType = l.LocationType.Name,
                        DateOfFirstConsent = p.DateOfFirstConsent,
                        InitialAssessmentCompletedOn = (
                            from a in db.ParticipantAssessments
                            where a.ParticipantId == p.Id
                            orderby a.Created ascending
                            select a
                        ).First().Completed!.Value,
                        AssessmentId = context.Message.Id,
                        Completed = context.Message.OccurredOn,
                        Created = a.Created!.Value,
                        ParticipantId = a.ParticipantId,
                        TenantId = a.TenantId!,
                        SupportWorker = a.CompletedBy!
                    };

        return await query.SingleAsync();
    }


    public record Data
    {
        public required Guid AssessmentId { get; set; }
        public required DateTime Completed { get; set; }

        public DateTime PeriodFrom => new(Completed.AddMonths(-2).Year, Completed.AddMonths(-2).Month, day: 1);
        public bool WasInitialAssessmentCompletedInLastTwoPaymentMonths => InitialAssessmentCompletedOn > PeriodFrom;
        public bool HaveAnyPaymentsBeenMadeInLastTwoPaymentMonths => PreviouslyPaidAssessments.Any(a => a.PaidOn >= PeriodFrom);

        public required DateTime InitialAssessmentCompletedOn { get; set; }
        public IEnumerable<Assessment> PreviouslyPaidAssessments { get; set; } = [];
        public required DateTime Created { get; set; }
        public required string ParticipantId { get; set; }
        public required string TenantId { get; set; }
        public required string SupportWorker { get; set; }
        public required string ContractId { get; set; }
        public required int LocationId { get; set; }
        public required string LocationType { get; set; }
        public required DateOnly? DateOfFirstConsent { get; init; }

        public record Assessment(DateTime PaidOn);
    }
}
