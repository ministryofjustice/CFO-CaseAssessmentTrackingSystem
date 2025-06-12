using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payments.Queries;

public static class GetEmploymentPayments
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : IRequest<Result<EmploymentPaymentsDto>>
    {
        public required int Month { get; set; }
        public required int Year { get; set; }
        public required string TenantId { get; set; }
        public string? ContractId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ITargetsProvider targetsProvider) : IRequestHandler<Query, Result<EmploymentPaymentsDto>>
    {
        public async Task<Result<EmploymentPaymentsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            EmploymentPaymentsDto result = new();

            var query = from ep in unitOfWork.DbContext.EmploymentPayments
                        join dd in unitOfWork.DbContext.DateDimensions on ep.PaymentPeriod equals dd.TheDate
                        join c in unitOfWork.DbContext.Contracts on ep.ContractId equals c.Id
                        join l in unitOfWork.DbContext.Locations on ep.LocationId equals l.Id
                        join a in unitOfWork.DbContext.EmploymentActivities on ep.ActivityId equals a.Id
                        where dd.TheMonth == request.Month && dd.TheYear == request.Year
                        select new
                        {
                            ep.CreatedOn,
                            a.CommencedOn,
                            ep.ActivityApproved,
                            ep.ParticipantId,
                            ep.EligibleForPayment,
                            ep.LocationType,
                            Location = l.Name,
                            Contract = c.Description,
                            ContractId = c.Id,
                            ep.IneligibilityReason,
                            TenantId = c!.Tenant!.Id!,
                            ParticipantName = a.Participant!.FirstName + " " + a.Participant!.LastName,
                            ep.PaymentPeriod,
                            a.EmploymentType,
                            EmploymentCategory = a.Definition.Category.Name
                        };

            query = request.ContractId is null
                ? query.Where(q => q.TenantId.StartsWith(request.TenantId))
                : query.Where(q => q.ContractId == request.ContractId);

            result.Items = await query.AsNoTracking()
                .Select(x => new EmploymentPaymentDto
                {
                    CreatedOn = x.CreatedOn,
                    CommencedOn = x.CommencedOn,
                    ActivityApproved = x.ActivityApproved,
                    ParticipantId = x.ParticipantId,
                    EligibleForPayment = x.EligibleForPayment,
                    Contract = x.Contract,
                    Location = x.Location,
                    LocationType = x.LocationType,
                    ParticipantName = x.ParticipantName,
                    PaymentPeriod = x.PaymentPeriod,
                    IneligibilityReason = x.IneligibilityReason,
                    EmploymentCategory = x.EmploymentCategory,
                    EmploymentType = x.EmploymentType
                })
                .OrderBy(e => e.Contract)
                .ThenByDescending(e => e.CreatedOn)
                .ToArrayAsync(cancellationToken);

            result.ContractSummary = result.Items
                .Where(e => e.EligibleForPayment)
                .GroupBy(e => e.Contract)
                .Select(x => new EmploymentPaymentSummaryDto
                {
                    Contract = x.Key,
                    Employments = x.Count(),
                    EmploymentsTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).Employment
                })
                .OrderBy(c => c.Contract)
                .ToList();

            return Result<EmploymentPaymentsDto>.Success(result);
        }
    }

    public class Validator() : AbstractValidator<Query>
    {

    }

}
