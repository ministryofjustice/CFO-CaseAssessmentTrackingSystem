using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PRIs.Commands;

public static class AddActualReleaseDate
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]

    public class Command : IRequest<Result>
    {
        [Description("Participant Id")]
        public required string ParticipantId { get; set; }

        [Description("Actual Release Date")]
        public DateTime? ActualReleaseDate { get; set; }         
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {               
            var pri = await unitOfWork.DbContext.PRIs
                .SingleOrDefaultAsync(p => p.ParticipantId == request.ParticipantId 
                && p.ActualReleaseDate == null 
                && p.IsCompleted==false, cancellationToken);

            if (pri == null)
            {
                throw new NotFoundException("Cannot find PRI", request.ParticipantId);
            }

            pri.SetActualReleaseDate(DateOnly.FromDateTime(request.ActualReleaseDate!.Value));
            
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

            RuleFor(c => c.ActualReleaseDate)
                .NotNull()
                .WithMessage("Actual Release Date is required")
                .LessThanOrEqualTo(DateTime.Today)
                .WithMessage(ValidationConstants.DateMustBeInPast);
        }
    }

}