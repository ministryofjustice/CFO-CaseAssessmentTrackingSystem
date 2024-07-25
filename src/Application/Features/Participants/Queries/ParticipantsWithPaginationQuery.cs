using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.Features.Participants.Specifications;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class ParticipantsWithPagination
{
    [RequestAuthorize(Policy = PolicyNames.AllowCandidateSearch)]
    public class Query : ParticipantAdvancedFilter, ICacheableRequest<PaginatedData<ParticipantDto>>
    {

        public ParticipantAdvancedSpecification Specification => new(this);
    
        public string CacheKey => ParticipantCacheKey.GetCacheKey($"{this}");
        public MemoryCacheEntryOptions? Options => ParticipantCacheKey.MemoryCacheEntryOptions;

        public override string ToString() =>
            $"ListView:{ListView}, Search:{Keyword}, {OrderBy}, {SortDirection}, {PageNumber}, {CurrentUser!.UserId}";
    }
    
    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, PaginatedData<ParticipantDto>>
    {
        public async Task<PaginatedData<ParticipantDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var data = await unitOfWork.DbContext.Participants.OrderBy($"{request.OrderBy} {request.SortDirection}")
                .ProjectToPaginatedDataAsync<Participant, ParticipantDto>(request.Specification, request.PageNumber, request.PageSize, mapper.ConfigurationProvider, cancellationToken);
            return data;
        }
    }
    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(r => r.Keyword)
                .Matches(RegularExpressionValidation.Keyword)
                .WithMessage(string.Format(RegularExpressionValidation.KeywordMessage, "Search Keyword"));

            RuleFor(r => r.PageNumber)
                .GreaterThan(0)
                .WithMessage(string.Format(RegularExpressionValidation.PositiveNumberMessage, "Page Number"));

            RuleFor(r => r.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000)
                .WithMessage(string.Format(RegularExpressionValidation.PageSizeMessage, "Page Size"));

            RuleFor(r => r.SortDirection)
                .Matches(RegularExpressionValidation.SortDirection)
                .WithMessage(RegularExpressionValidation.SortDirectionMessage);

            //May be at some point in future validate against columns of query result dataset
            RuleFor(r => r.OrderBy)
                .Matches(RegularExpressionValidation.AlphaNumeric)
                .WithMessage(string.Format(RegularExpressionValidation.AlphaNumericMessage, "OrderBy"));

        }
    }
}





