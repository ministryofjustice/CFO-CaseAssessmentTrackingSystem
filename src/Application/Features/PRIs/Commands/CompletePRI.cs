using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Commands;

public static class CompletePRI
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]

    public class Command : IRequest<Result>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }

        [Description("Completed By")]
        public required string? CompletedBy { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var pri = await unitOfWork.DbContext.PRIs
                .SingleOrDefaultAsync(p => p.ParticipantId == request.ParticipantId
                && p.Status == PriStatus.Accepted, cancellationToken);

            if (pri == null)
            {
                throw new NotFoundException("Cannot find PRI", request.ParticipantId);
            }

            pri.Complete(request.CompletedBy);

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


            RuleFor(c => c.CompletedBy)
                    .NotNull()
                    .MinimumLength(36);
        }
    }

    public class B_EntryMustHaveActualReleaseDate : AbstractValidator<Command>
    {
        private readonly IUnitOfWork _unitOfWork;

        public B_EntryMustHaveActualReleaseDate(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            RuleFor(c => c.ParticipantId)
                .MustAsync(ActualReleaseDateMustExist)
                .WithMessage("Actual Release Date Required");
        }
        private async Task<bool> ActualReleaseDateMustExist(string identifier, CancellationToken cancellationToken)
            => await _unitOfWork.DbContext.PRIs.AnyAsync(p => p.ParticipantId == identifier && p.ActualReleaseDate != null, cancellationToken);
    }
}