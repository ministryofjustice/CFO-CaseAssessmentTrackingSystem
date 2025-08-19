using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Mappings;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;
using Newtonsoft.Json;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class ParticipantsWithPagination
{
    [RequestAuthorize(Policy = SecurityPolicies.CandidateSearch)]
    public class Query : ParticipantAdvancedFilter, IRequest<Result<PaginatedData<ParticipantPaginationDto>>>
    {
        [JsonIgnore]
        public ParticipantAdvancedSpecification Specification => new(this);
    
    }
    
    public class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, Result<PaginatedData<ParticipantPaginationDto>>>
    {
        public async Task<Result<PaginatedData<ParticipantPaginationDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.Participants.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<Participant, ParticipantPaginationDto>(request.Specification, request.PageNumber, request.PageSize, ParticipantMappings.ToPaginationDto, cancellationToken);
            return data;
        }
    }
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.Keyword)
                .Matches(ValidationConstants.Keyword)
                .WithMessage(string.Format(ValidationConstants.KeywordMessage, "Search Keyword"));

            RuleFor(r => r.PageNumber)
                .GreaterThan(0)
                .WithMessage(string.Format(ValidationConstants.PositiveNumberMessage, "Page Number"));

            RuleFor(r => r.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(ValidationConstants.MaximumPageSize)
                .WithMessage(ValidationConstants.MaximumPageSizeMessage);

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

