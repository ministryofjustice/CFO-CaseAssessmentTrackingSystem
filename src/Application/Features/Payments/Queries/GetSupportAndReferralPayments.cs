using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payments.Queries;

public static class GetSupportAndReferralPayments
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : BaseFilter, IRequest<Result<SupportAndReferralPaymentsDto>>
    {
        public required int Month { get; set; }
        public required int Year { get; set; }
        public required string TenantId { get; set; }
        public string? ContractId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ITargetsProvider targetsProvider) : IRequestHandler<Query, Result<SupportAndReferralPaymentsDto>>
    {
        public async Task<Result<SupportAndReferralPaymentsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            SupportAndReferralPaymentsDto result = new();

            var query = from ep in unitOfWork.DbContext.SupportAndReferralPayments
                        join dd in unitOfWork.DbContext.DateDimensions on ep.PaymentPeriod equals dd.TheDate
                        join c in unitOfWork.DbContext.Contracts on ep.ContractId equals c.Id
                        join l in unitOfWork.DbContext.Locations on ep.LocationId equals l.Id
                        join p in unitOfWork.DbContext.Participants on ep.ParticipantId equals p.Id
                        where dd.TheMonth == request.Month && dd.TheYear == request.Year
                        select new
                        {
                            ep.ActivityInput,
                            ep.CreatedOn,
                            ep.Approved,
                            ep.ParticipantId,
                            PaymentDate = ep.PaymentPeriod,
                            ep.EligibleForPayment,
                            ep.SupportType,
                            Contract = c.Description,
                            ContractId = c.Id,
                            ep.LocationType,
                            Location = l.Name,
                            ep.IneligibilityReason,
                            TenantId = c!.Tenant!.Id!,
                            ParticipantName = p.FirstName + " " + p.LastName
                        };

            query = request.ContractId is null
                ? query.Where(q => q.TenantId.StartsWith(request.TenantId))
                : query.Where(q => q.ContractId == request.ContractId);

            if (string.IsNullOrWhiteSpace(request.Keyword) is false)
            {
                query = query.Where(
                    x => x.ParticipantName.Contains(request.Keyword)
                      || x.ParticipantId.Contains(request.Keyword)
                      || x.IneligibilityReason != null && x.IneligibilityReason.Contains(request.Keyword)
                      || x.Location.Contains(request.Keyword)
                      || x.LocationType.Contains(request.Keyword));
            }

            result.Items = await query.AsNoTracking()
                .Select(x => new SupportAndReferralPaymentDto
                {
                    CreatedOn = x.CreatedOn,
                    Approved = x.Approved,
                    ParticipantId = x.ParticipantId,
                    EligibleForPayment = x.EligibleForPayment,
                    Contract = x.Contract,
                    SupportType = x.SupportType,
                    Location = x.Location,
                    LocationType = x.LocationType,
                    IneligibilityReason = x.IneligibilityReason,
                    ParticipantName = x.ParticipantName,
                    PaymentPeriod = x.PaymentDate
                })
                .OrderBy(e => e.Contract)
                .ThenByDescending(e => e.CreatedOn)
                .ToArrayAsync();

            result.ContractSummary = result.Items
                .Where(e => e.EligibleForPayment)
                .GroupBy(e => e.Contract)
                .Select(x => new SupportAndReferralPaymentSummaryDto
                {
                    Contract = x.Key,
                    PreReleaseSupport = x.Count(g => g.SupportType == "Pre-Release Support"),
                    PreReleaseSupportTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).PreReleaseSupport,
                    ThroughTheGateSupport = x.Count(g => g.SupportType == "Through the Gate"),
                    ThroughTheGateSupportTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).ThroughTheGate,
                })
                .OrderBy(c => c.Contract)
                .ToList();

            return Result<SupportAndReferralPaymentsDto>.Success(result);
        }
    }

    public class Validator() : AbstractValidator<Query>
    {

    }

}
