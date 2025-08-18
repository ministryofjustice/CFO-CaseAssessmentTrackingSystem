using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payments.Queries;

public static class GetEnrolmentPayments
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : BaseFilter, IRequest<Result<EnrolmentPaymentsDto>>
    {
        public required int Month { get; set; }
        public required int Year { get; set; }
        public required string TenantId { get; set; }
        public string? ContractId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ITargetsProvider targetsProvider) : IRequestHandler<Query, Result<EnrolmentPaymentsDto>>
    {
        public async Task<Result<EnrolmentPaymentsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            EnrolmentPaymentsDto result = new();

            var query = from ep in unitOfWork.DbContext.EnrolmentPayments
                        join dd in unitOfWork.DbContext.DateDimensions on ep.Approved equals dd.TheDate
                        join c in unitOfWork.DbContext.Contracts on ep.ContractId equals c.Id
                        join l in unitOfWork.DbContext.Locations on ep.LocationId equals l.Id
                        join p in unitOfWork.DbContext.Participants on ep.ParticipantId equals p.Id
                        where dd.TheMonth == request.Month && dd.TheYear == request.Year
                        select new
                        {
                            ep.CreatedOn,
                            ep.Approved,
                            ep.ParticipantId,
                            ep.EligibleForPayment,
                            Contract = c.Description,
                            ContractId = c.Id,
                            ep.LocationType,
                            Location = l.Name,
                            ep.IneligibilityReason,
                            TenantId = c!.Tenant!.Id!,
                            ParticipantName = p.FirstName + " " + p.LastName,
                            ep.SubmissionToAuthority
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
                .Select(x => new EnrolmentPaymentDto
                {
                    CreatedOn = x.CreatedOn,
                    Approved = x.Approved,
                    ParticipantId = x.ParticipantId,
                    EligibleForPayment = x.EligibleForPayment,
                    Contract = x.Contract,
                    Location = x.Location,
                    LocationType = x.LocationType,
                    IneligibilityReason = x.IneligibilityReason,
                    ParticipantName = x.ParticipantName,
                    SubmissionToAuthority = x.SubmissionToAuthority
                })
                .OrderBy(e => e.Contract)
                .ThenByDescending(e => e.CreatedOn)
                .ToArrayAsync(cancellationToken);

            result.ContractSummary = result.Items
                .Where(e => e.EligibleForPayment)
                .GroupBy(e => e.Contract)
                .Select(x => new EnrolmentPaymentSummaryDto
                {
                    Contract = x.Key,
                    Custody = x.Count(g => g.IsCustody()),
                    Community = x.Count(g => g.IsCustody() == false),
                    CustodyTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).Prison,
                    CommunityTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).Community
                })
                .OrderBy(c => c.Contract)
                .ToList();

            return Result<EnrolmentPaymentsDto>.Success(result);
        }
    }

    public class Validator() : AbstractValidator<Query>
    {

    }

}
