using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.Caching;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetParticipantSummary
{
    [RequestAuthorize(Policy = PolicyNames.AllowCandidateSearch)]
    public class Query : ICacheableRequest<Result<ParticipantSummaryDto>>
    {
        public required string ParticipantId { get; set; } 
        public required UserProfile CurrentUser { get; set; }
        
        public string CacheKey => ParticipantCacheKey.GetCacheKey($"ParticipantSummary,{ParticipantId}");
        public MemoryCacheEntryOptions? Options => ParticipantCacheKey.MemoryCacheEntryOptions;
        
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<ParticipantSummaryDto>>
    {
        
        public async Task<Result<ParticipantSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = from c in unitOfWork.DbContext.Participants
                where c.Id == request.ParticipantId
                select c;

            var summary = await query.ProjectTo<ParticipantSummaryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(cancellationToken);

            if (summary == null)
            {
                throw new NotFoundException(nameof(ParticipantSummaryDto), request.ParticipantId);
            }

            summary.Assessments = await unitOfWork.DbContext.ParticipantAssessments
                .Where(pa => pa.ParticipantId == request.ParticipantId)
                .ProjectTo<AssessmentSummaryDto>(mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);
                
            return Result<ParticipantSummaryDto>.Success(summary);

        }
    }

    public class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull();

            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "ParticipantId"));
        }
    }
}