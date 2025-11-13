using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public class GetParticipantRisk
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<Result<RiskDto>>
    {
        public required string ParticipantId { get; set; }
        public Guid? RiskId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<RiskDto>>
    {
        public async Task<Result<RiskDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query =
                from r in unitOfWork.DbContext.Risks
                    .Include(x => x.Participant)
                    .IgnoreAutoIncludes()
                join l in unitOfWork.DbContext.Locations
                    on r.LocationId equals l.Id
                where r.ParticipantId == request.ParticipantId
                select new { Risk = r, LocationType = l.LocationType };

            if (request.RiskId is not null)
            {
                query = query
                    .Where(x => x.Risk.Id == request.RiskId);
            }

            var result = await query
                .OrderByDescending(x => x.Risk.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if(result is null)
            {
                return Result<RiskDto>.Failure(["Risk not found."]);
            }

            var dto = mapper.Map<RiskDto>(result.Risk);
            dto.LocationType = result.LocationType;

            return dto;
        }

    }  

    public class Validator : AbstractValidator<Query>
    {
        private readonly IUnitOfWork _unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .NotNull()
                .Length(9)
                .WithMessage("Invalid Participant Id")
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            When(x => x.RiskId is not null, () => {
                RuleFor(x => x.RiskId)
                    .NotEmpty();
            });

            RuleSet(ValidationConstants.RuleSet.MediatR, () =>
            {
                RuleFor(c => c.ParticipantId)
                    .MustAsync(Exist)
                    .WithMessage("Participant does not exist");
            });
        }

        private async Task<bool> Exist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.Participants.AnyAsync(e => e.Id == identifier, cancellationToken);
    }
}
