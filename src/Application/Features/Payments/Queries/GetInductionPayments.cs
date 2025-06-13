using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payments.Queries;

public static class GetInductionPayments
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : BaseFilter, IRequest<Result<InductionPaymentsDto>>
    {
        public required int Month { get; set; }
        public required int Year { get; set; }
        public required string TenantId { get; set; }
        public string? ContractId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ITargetsProvider targetsProvider) : IRequestHandler<Query, Result<InductionPaymentsDto>>
    {
        public async Task<Result<InductionPaymentsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            InductionPaymentsDto result = new();

            var query = from ip in unitOfWork.DbContext.InductionPayments
                        join dd in unitOfWork.DbContext.DateDimensions on ip.PaymentPeriod equals dd.TheDate
                        join c in unitOfWork.DbContext.Contracts on ip.ContractId equals c.Id
                        join l in unitOfWork.DbContext.Locations on ip.LocationId equals l.Id
                        join p in unitOfWork.DbContext.Participants on ip.ParticipantId equals p.Id
                        where dd.TheMonth == request.Month && dd.TheYear == request.Year
                        select new
                        {
                            ip.InductionInput,
                            ip.CommencedDate,
                            ip.Approved,
                            ip.ParticipantId,
                            ip.EligibleForPayment,
                            Contract = c.Description,
                            ContractId = c.Id,
                            ip.LocationType,
                            Location = l.Name,
                            ip.IneligibilityReason,
                            TenantId = c!.Tenant!.Id!,
                            ip.PaymentPeriod,
                            ip.Induction,
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
                .Select(x => new InductionPaymentDto
                {
                    CreatedOn = x.InductionInput,
                    InductionDate = x.Induction,
                    CommencedOn = x.CommencedDate,
                    Approved = x.Approved,
                    PaymentPeriod = x.PaymentPeriod,
                    ParticipantId = x.ParticipantId,
                    EligibleForPayment = x.EligibleForPayment,
                    Contract = x.Contract,
                    Location = x.Location,
                    LocationType = x.LocationType,
                    IneligibilityReason = x.IneligibilityReason,
                    ParticipantName = x.ParticipantName
                })
                .OrderBy(e => e.Contract)
                .ThenByDescending(e => e.CreatedOn)
                .ToArrayAsync(cancellationToken);

            result.ContractSummary = result.Items
                .Where(e => e.EligibleForPayment)
                .GroupBy(e => e.Contract)
                .Select(x => new InductionPaymentSummaryDto
                {
                    Contract = x.Key,
                    Wings = x.Count(g => g.LocationType == "Wing"),
                    Hubs = x.Count(g => g.LocationType == "Hub"),
                    WingsTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).Wings,
                    HubsTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).Hubs,
                })
                .OrderBy(c => c.Contract)
                .ToList();

            return Result<InductionPaymentsDto>.Success(result);
        }
    }

    public class Validator() : AbstractValidator<Query>
    {

    }

}
