using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Commands;

public static class AbandonPRI
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]

    public class Command : IRequest<Result>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }

        [Description("Reason Abandoned")]
        public string? ReasonAbandoned { get; set; }

        [Description("Abandoned By")]
        public string? AbandonedBy { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pri = await unitOfWork.DbContext.PRIs
                .SingleOrDefaultAsync(p => p.ParticipantId == request.ParticipantId
                && p.Status != PriStatus.Abandoned, cancellationToken);


            if (pri == null)
            {
                throw new NotFoundException("Cannot find PRI", request.ParticipantId);
            }            

            pri.Abandon(request.ReasonAbandoned,request.AbandonedBy);

            return Result.Success();
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.ParticipantId)
                .Length(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(c => c.AbandonedBy)
                    .NotNull()
                    .MinimumLength(36);

            RuleFor(c => c.ReasonAbandoned)
             .NotEmpty()             
             .WithMessage("You must provide a reason");
        }
    }
}