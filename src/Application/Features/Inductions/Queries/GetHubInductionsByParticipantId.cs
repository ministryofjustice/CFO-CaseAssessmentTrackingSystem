using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Inductions.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Inductions.Queries;

public static class GetHubInductionsByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<Result<HubInductionDto[]>>
    { 
        public string? ParticipantId { get; set; }
    }
    
    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<HubInductionDto[]>>
    {
        public async Task<Result<HubInductionDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.DbContext.HubInductions.AsNoTracking()
                .Where(x => x.ParticipantId == request.ParticipantId!)
                .ProjectTo<HubInductionDto>(mapper.ConfigurationProvider);

            return await query.ToArrayAsync(cancellationToken);
        }
    }

    public class Validator : AbstractValidator<Query>
    { 
        public Validator() 
        {
            RuleFor(x => x.ParticipantId)
                .NotNull()
                .MaximumLength(9)
                .MinimumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }
    }
}
