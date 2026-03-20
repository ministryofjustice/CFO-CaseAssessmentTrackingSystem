using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Timelines.DTOs;
using Cfo.Cats.Application.Features.Timelines.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Timelines.PaginationQuery;

public static class TimelinesWithPaginationQuery
{
    [RequestAuthorize(Policy = SecurityPolicies.CandidateSearch)]
    public class Query : TimelineAdvancedFilter, IRequest<Result<PaginatedData<TimelineDto>>>
    {
        public TimelineAdvancedSpecification Specification => new(this);
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<PaginatedData<TimelineDto>>>
    {
        public async Task<Result<PaginatedData<TimelineDto>>> Handle(Query request, CancellationToken cancellationToken) 
            => await unitOfWork.DbContext.Timelines
                .OrderBy("Created DESC")
                .ProjectToPaginatedDataAsync<Timeline, TimelineDto>(
                    request.Specification,
                    request.PageNumber,
                    request.PageSize,
                    mapper.ConfigurationProvider,
                    cancellationToken
                );
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.ParticipantId)
                .NotNull()
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(r => r.Keyword)
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Search Keyword"));
            
            RuleFor(r => r.PageNumber)
                .GreaterThan(0)
                .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Page Number"));

            RuleFor(r => r.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000)
                .WithMessage(string.Format(ValidationConstants.MaximumPageSizeMessage, "Page Size"));

            RuleFor(r => r.SortDirection)
                .Matches(ValidationConstants.SortDirection)
                .WithMessage(ValidationConstants.SortDirectionMessage);
            
            //May be at some point in future validate against columns of query result dataset
            RuleFor(r => r.OrderBy)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "OrderBy"));

        }
    }
    

}
    
