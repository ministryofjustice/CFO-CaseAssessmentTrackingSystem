using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payments.Queries;

public static class GetActivityPayments
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : BaseFilter, IRequest<Result<ActivityPaymentsDto>>
    {
        public required int Month { get; set; }
        public required int Year { get; set; }
        public required string TenantId { get; set; }
        public string? ContractId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ITargetsProvider targetsProvider) : IRequestHandler<Query, Result<ActivityPaymentsDto>>
    {
        public async Task<Result<ActivityPaymentsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            ActivityPaymentsDto result = new();

            var query =
            (
                from ep in unitOfWork.DbContext.ActivityPayments
                join dd in unitOfWork.DbContext.DateDimensions on ep.PaymentPeriod equals dd.TheDate
                join c in unitOfWork.DbContext.Contracts on ep.ContractId equals c.Id
                join l in unitOfWork.DbContext.Locations on ep.LocationId equals l.Id
                join p in unitOfWork.DbContext.Participants on ep.ParticipantId equals p.Id
                where dd.TheMonth == request.Month && dd.TheYear == request.Year
                select new
                {
                    ep.ActivityInput,
                    ep.CommencedDate,
                    ep.ActivityApproved,
                    PaymentDate = ep.PaymentPeriod,
                    ep.ParticipantId,
                    ep.EligibleForPayment,
                    ep.ActivityCategory,
                    ep.ActivityType,
                    Contract = c.Description,
                    ContractId = c.Id,
                    ep.LocationType,
                    Location = l.Name,
                    ep.IneligibilityReason,
                    ep.PaymentPeriod,
                    TenantId = c!.Tenant!.Id!,
                    ParticipantName = p.FirstName + " " + p.LastName
                }
            )
            .Union
            (
                    from re in unitOfWork.DbContext.ReassessmentPayments
                    join dd in unitOfWork.DbContext.DateDimensions on re.PaymentPeriod equals dd.TheDate
                    join c in unitOfWork.DbContext.Contracts on re.ContractId equals c.Id
                    join l in unitOfWork.DbContext.Locations on re.LocationId equals l.Id
                    join p in unitOfWork.DbContext.Participants on re.ParticipantId equals p.Id
                    where dd.TheMonth == request.Month && dd.TheYear == request.Year
                    select new
                    {
                        ActivityInput = re.AssessmentCreated, // AssessmentCreated
                        CommencedDate = re.AssessmentCompleted, // AssessmentCreated? 
                        ActivityApproved = re.AssessmentCompleted,
                        PaymentDate = re.PaymentPeriod,
                        re.ParticipantId,
                        re.EligibleForPayment,
                        ActivityCategory = "Participant Reassessment",
                        ActivityType = ActivityType.SupportWork.Name,
                        Contract = c.Description,
                        ContractId = c.Id,
                        re.LocationType,
                        Location = l.Name,
                        re.IneligibilityReason,
                        re.PaymentPeriod,
                        TenantId = c!.Tenant!.Id!,
                        ParticipantName = p.FirstName + " " + p.LastName
                    }
            );

            query = request.ContractId is null
                ? query.Where(q => q.TenantId.StartsWith(request.TenantId))
                : query.Where(q => q.ContractId == request.ContractId);

            if(string.IsNullOrWhiteSpace(request.Keyword) is false)
            {
                query = query.Where(
                    x => x.ParticipantName.Contains(request.Keyword)
                      || x.ParticipantId.Contains(request.Keyword)
                      || x.IneligibilityReason != null && x.IneligibilityReason.Contains(request.Keyword)
                      || x.ActivityCategory.Contains(request.Keyword)
                      || x.Location.Contains(request.Keyword)
                      || x.LocationType.Contains(request.Keyword));
            }

            result.Items = await query.AsNoTracking()
                .Select(x => new ActivityPaymentDto
                {
                    CreatedOn = x.ActivityInput,
                    CommencedOn = x.CommencedDate,
                    ActivityApproved = x.ActivityApproved,
                    ParticipantId = x.ParticipantId,
                    EligibleForPayment = x.EligibleForPayment,
                    Contract = x.Contract,
                    Category = x.ActivityCategory,
                    Type = x.ActivityType,
                    Location = x.Location,
                    LocationType = x.LocationType,
                    IneligibilityReason = x.IneligibilityReason,
                    ParticipantName = x.ParticipantName,
                    PaymentPeriod = x.PaymentPeriod
                })
                .OrderBy(e => e.Contract)
                .ThenByDescending(e => e.CreatedOn)
                .ToArrayAsync(cancellationToken);

            result.ContractSummary = result.Items
                .Where(e => e.EligibleForPayment)
                .GroupBy(e => e.Contract)
                .Select(x => new ActivityPaymentSummaryDto
                {
                    Contract = x.Key,
                    SupportWork = x.Count(g => g.Type == ActivityType.SupportWork.Name),
                    SupportWorkTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).SupportWork,
                    CommunityAndSocial = x.Count(g => g.Type == ActivityType.CommunityAndSocial.Name),
                    CommunityAndSocialTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).CommunityAndSocial,
                    HumanCitizenship = x.Count(g => g.Type == ActivityType.HumanCitizenship.Name),
                    HumanCitizenshipTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).HumanCitizenship,
                    ISWSupport = x.Count(g => g.Type == ActivityType.InterventionsAndServicesWraparoundSupport.Name),
                    ISWSupportTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).Interventions
                })
                .OrderBy(c => c.Contract)
                .ToList();

            return Result<ActivityPaymentsDto>.Success(result);
        }
    }

    public class Validator() : AbstractValidator<Query>
    {

    }

}
