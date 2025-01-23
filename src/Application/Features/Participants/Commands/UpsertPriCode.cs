using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Participants;

namespace Cfo.Cats.Application.Features.Participants.Commands;

public static class UpsertPriCode
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Command : IRequest<Result<int>>
    {
        [Description("Participant Id")]
        public string ParticipantId { get; set; } = string.Empty;
    }
    
    public class Handler(IUnitOfWork unitOfWork, ICurrentUserService userService) : IRequestHandler<Command, Result<int>>
    {
        public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
        {
            var priCode = await unitOfWork.DbContext.PriCodes.FindAsync(
                [request.ParticipantId, userService.UserId], 
                cancellationToken);

            if(priCode is not null)
            {
                unitOfWork.DbContext.PriCodes.Remove(priCode);
            }

            priCode = PriCode.Create(request.ParticipantId, userService.UserId!);
            await unitOfWork.DbContext.PriCodes.AddAsync(priCode, cancellationToken);

            return Result<int>.Success(priCode.Code);
        }

    }

    public class Validator : AbstractValidator<Command>
    {
        readonly IUnitOfWork unitOfWork;

        public Validator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

            RuleFor(x => x.ParticipantId)
                .NotEmpty()
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"))
                .MustAsync(Exist)
                .WithMessage("Participant not found");

        }
        async Task<bool> Exist(string participantId, CancellationToken cancellationToken)
            => await unitOfWork.DbContext.Participants.AnyAsync(p => p.Id == participantId, cancellationToken);
    }
}
