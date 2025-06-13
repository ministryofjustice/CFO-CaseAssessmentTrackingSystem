using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.ManagementInformation;
using Cfo.Cats.Application.Features.Payments.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payments.Queries;

public static class GetEducationPayments
{
    [RequestAuthorize(Roles = $"{RoleNames.SystemSupport}, {RoleNames.Finance}")]
    public class Query : BaseFilter, IRequest<Result<EducationPaymentsDto>>
    {
        public required int Month { get; set; }
        public required int Year { get; set; }
        public required string TenantId { get; set; }
        public string? ContractId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, ITargetsProvider targetsProvider) : IRequestHandler<Query, Result<EducationPaymentsDto>>
    {
        public async Task<Result<EducationPaymentsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            EducationPaymentsDto result = new();

            var query = from ep in unitOfWork.DbContext.EducationPayments
                        join dd in unitOfWork.DbContext.DateDimensions on ep.PaymentPeriod equals dd.TheDate
                        join c in unitOfWork.DbContext.Contracts on ep.ContractId equals c.Id
                        join l in unitOfWork.DbContext.Locations on ep.LocationId equals l.Id
                        join a in unitOfWork.DbContext.EducationTrainingActivities on ep.ActivityId equals a.Id
                        where dd.TheMonth == request.Month && dd.TheYear == request.Year
                        select new
                        {
                            ep.ActivityInput,
                            a.CommencedOn,
                            ep.ActivityApproved,
                            ep.ParticipantId,
                            ep.EligibleForPayment,
                            Contract = c.Description,
                            ep.LocationType,
                            ContractId = c.Id,
                            Location = l.Name,
                            ep.IneligibilityReason,
                            TenantId = c!.Tenant!.Id!,
                            ep.PaymentPeriod,
                            ParticipantName = a.Participant!.FirstName + " " + a.Participant!.LastName,
                            a.CourseLevel,
                            a.CourseTitle
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
                .Select(x => new EducationPaymentDto
                {
                    CreatedOn = x.ActivityInput,
                    CommencedOn = x.CommencedOn,
                    ActivityApproved = x.ActivityApproved,
                    ParticipantId = x.ParticipantId,
                    EligibleForPayment = x.EligibleForPayment,
                    Contract = x.Contract,
                    Location = x.Location,
                    LocationType = x.LocationType,
                    IneligibilityReason = x.IneligibilityReason,
                    ParticipantName = x.ParticipantName,
                    PaymentPeriod = x.PaymentPeriod,
                    CourseLevel = x.CourseLevel,
                    CourseTitle = x.CourseTitle
                })
                .OrderBy(e => e.Contract)
                .ThenByDescending(e => e.CreatedOn)
                .ToArrayAsync(cancellationToken);

            result.ContractSummary = result.Items
                .Where(e => e.EligibleForPayment)
                .GroupBy(e => e.Contract)
                .Select(x => new EducationPaymentSummaryDto
                {
                    Contract = x.Key,
                    Educations = x.Count(),
                    EducationsTarget = targetsProvider.GetTarget(x.Key, request.Month, request.Year).TrainingAndEducation
                })
                .OrderBy(c => c.Contract)
                .ToList();

            return Result<EducationPaymentsDto>.Success(result);
        }
    }

    public class Validator() : AbstractValidator<Query>
    {

    }

}
