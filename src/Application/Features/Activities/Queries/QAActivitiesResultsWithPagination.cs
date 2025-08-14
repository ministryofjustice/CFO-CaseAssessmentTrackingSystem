using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Activities.DTOs;
using Cfo.Cats.Application.Features.Activities.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Activities;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class QAActivitiesResultsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : QAActivitiesResultsAdvancedFilter, IRequest<PaginatedData<QAActivitiesResultsSummaryDto>>
    {
        public QAActivitiesResultsAdvancedSpecification Specification => new(this);
    }

    class Handler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<Query, PaginatedData<QAActivitiesResultsSummaryDto>>
    {
        public async Task<PaginatedData<QAActivitiesResultsSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.Activities
                .Include(a => a.TookPlaceAtLocation)
                .Include(a => a.Participant)
                .ToListAsync(cancellationToken);

            var filtered = data.Where(a => a.RequiresQa && (a.Status == ActivityStatus.PendingStatus || a.Status == ActivityStatus.ApprovedStatus));

            return filtered
                  .AsQueryable()
                  .OrderByDescending(a => a.Status == ActivityStatus.PendingStatus.Value)
                  .ThenBy($"{request.OrderBy} {request.SortDirection}")
                  .ProjectToPaginatedData<Activity, QAActivitiesResultsSummaryDto>(
                      request.Specification,
                      request.PageNumber,
                      request.PageSize,
                      mapper.ConfigurationProvider
                  );
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.CurrentUser)
                    .NotNull();
            }
        }
    }
}