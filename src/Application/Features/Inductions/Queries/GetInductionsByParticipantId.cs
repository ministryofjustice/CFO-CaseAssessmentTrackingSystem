using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Inductions.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Inductions;

namespace Cfo.Cats.Application.Features.Inductions.Queries;

public static class GetInductionsByParticipantId
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<InductionsDto>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<InductionsDto>>
    {
        public async Task<Result<InductionsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var hubs = await unitOfWork.DbContext.HubInductions
                .AsNoTracking()
                .Where(h => h.ParticipantId == request.ParticipantId)
                .ProjectTo<HubInductionDto>(mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);

            var wings = await unitOfWork.DbContext.WingInductions
                .AsNoTracking()
                .Where(h => h.ParticipantId == request.ParticipantId)
                .ProjectTo<WingInductionDto>(mapper.ConfigurationProvider)
                .ToArrayAsync(cancellationToken);

            var model = new InductionsDto(hubs, wings);
            return model;
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
