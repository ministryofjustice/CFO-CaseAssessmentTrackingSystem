using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.QualityAssurance.DTOs;
using Cfo.Cats.Application.Features.QualityAssurance.Specifications;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.QualityAssurance.Queries;

public static class GetQaOfficerStatsUsers
{
    [RequestAuthorize(Policy = SecurityPolicies.ServiceDeskManagement)]
    public class Query : IQuery<Result<List<QaOfficerDto>>>
    {
        public QaStatsProcess Process { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork) : IQueryHandler<Query, Result<List<QaOfficerDto>>>
    {
        public async Task<Result<List<QaOfficerDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var filter = new QaOfficerStatsFilter
            {
                Process = request.Process,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            var users = await QaOfficerStatsWithPagination.BuildQuery(unitOfWork.DbContext, filter)
                .Where(x => x.QaUserId != null && x.QaUser != null)
                .Select(x => new QaOfficerDto { UserId = x.QaUserId!, DisplayName = x.QaUser! })
                .Distinct()
                .OrderBy(x => x.DisplayName)
                .ToListAsync(cancellationToken);

            return users;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.StartDate)
                .LessThanOrEqualTo(r => r.EndDate)
                .WithMessage("Start date must be on or before the end date.");

            RuleFor(r => r.EndDate.Date <= r.StartDate.Date.AddMonths(QaOfficerStatsFilter.MaxRangeMonths))
                .Equal(true)
                .WithMessage(QaOfficerStatsFilter.MaxRangeMessage);
        }
    }
}
