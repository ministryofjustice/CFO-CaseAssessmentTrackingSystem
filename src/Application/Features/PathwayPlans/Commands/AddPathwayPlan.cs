using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.PathwayPlans.Commands;

public static class AddPathwayPlan
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result>
    {
        public required string ParticipantId { get; set; }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Command, PathwayPlan>(MemberList.None)
                    .ConstructUsing(dto => PathwayPlan.Create(dto.ParticipantId));
            }
        }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            if (await unitOfWork.DbContext.PathwayPlans.AnyAsync(p => p.ParticipantId == request.ParticipantId))
            {
                return Result.Failure();
            }

            var pathwayPlan = mapper.Map<PathwayPlan>(request);

            await unitOfWork.DbContext.PathwayPlans.AddAsync(pathwayPlan);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .MinimumLength(9)
                .MaximumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));
        }

    }
}
