using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Identity.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetOwnerByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Result<ApplicationUserDto>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<ApplicationUserDto>>
    {
        public async Task<Result<ApplicationUserDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var participant = await unitOfWork.DbContext.Participants
                .Include(p => p.Owner)
                .SingleOrDefaultAsync(p => p.Id == request.ParticipantId, cancellationToken);

            if(participant is null)
            {
                return Result<ApplicationUserDto>.Failure();
            }

            return Result<ApplicationUserDto>.Success(mapper.Map<ApplicationUserDto>(participant.Owner));
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
