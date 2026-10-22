using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetOwnerByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<Result<ApplicationUserSummaryDto>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<Query, Result<ApplicationUserSummaryDto>>
    {
        public async Task<Result<ApplicationUserSummaryDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var exists = await unitOfWork.DbContext.Participants
                .AnyAsync(p => p.Id == request.ParticipantId, cancellationToken);

            if(!exists)
            {
                return Result<ApplicationUserSummaryDto>.Failure();
            }

            var owner = await unitOfWork.DbContext.Participants
                .Where(p => p.Id == request.ParticipantId && p.Owner != null)
                .Select(p => p.Owner!)
                .ProjectTo<ApplicationUserSummaryDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);

            return Result<ApplicationUserSummaryDto>.Success(owner!);
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
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }
    }
}
