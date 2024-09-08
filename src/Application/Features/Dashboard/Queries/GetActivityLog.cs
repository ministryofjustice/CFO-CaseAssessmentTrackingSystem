using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Dashboard.DTOs;
using Cfo.Cats.Application.Features.Dashboard.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace Cfo.Cats.Application.Features.Dashboard.Queries;

public static class GetActivityLog
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : ActivityLogFilter, IRequest<PaginatedData<ActivityLogDto>>
    {
        public ActivityLogSpecification Specification => new(this);
        
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<ActivityLogDto>>
    {
        public async Task<PaginatedData<ActivityLogDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.Timelines.AsNoTracking()
                .OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<Timeline, ActivityLogDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken );

            return data!;
        }
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
