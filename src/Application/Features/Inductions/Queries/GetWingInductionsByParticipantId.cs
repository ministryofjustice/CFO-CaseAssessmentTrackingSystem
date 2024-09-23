using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Inductions.DTOs;

namespace Cfo.Cats.Application.Features.Inductions.Queries;

public static class GetWingInductionsByParticipantId
{
    public class Query : IRequest<Result<WingInductionDto[]>>
    { 
        public string? ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<WingInductionDto[]>>
    {
        public async Task<Result<WingInductionDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.DbContext.HubInductions.AsNoTracking()
                .Where(x => x.ParticipantId == request.ParticipantId!)
                .ProjectTo<WingInductionDto>(mapper.ConfigurationProvider);

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
