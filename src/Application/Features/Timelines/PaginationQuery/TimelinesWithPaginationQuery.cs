using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Timelines.DTOs;
using Cfo.Cats.Application.Features.Timelines.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Timelines.PaginationQuery;

public static class TimelinesWithPaginationQuery
{

    [RequestAuthorize(Policy = PolicyNames.AllowCandidateSearch)]
    public class Query : TimelineAdvancedFilter, IRequest<PaginatedData<TimelineDto>>
    {
        public TimelineAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<TimelineDto>>
    {

        public async Task<PaginatedData<TimelineDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.Timelines
                .OrderBy("Created DESC")
                .ProjectToPaginatedDataAsync<Timeline, TimelineDto>(
                request.Specification,
                request.PageNumber,
                request.PageSize,
                mapper.ConfigurationProvider,
                cancellationToken
                );

            return data;
        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9);
        }
    }
    


}
    
