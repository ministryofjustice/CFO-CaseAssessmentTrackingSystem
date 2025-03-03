using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Candidates.Commands;

public static class SetCandidateStickyLocation
{
    [RequestAuthorize(Policy = SecurityPolicies.SeniorInternal)]
    public class Command : IRequest<Result>
    {
        public string? ParticipantId { get; set; }
        public string? Region { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.ParticipantId)
                .NotNull()
                .MaximumLength(9)
                .MinimumLength(9)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Participant Id"));

            RuleFor(x => x.Region)
                .NotNull()
                .MaximumLength(4)
                .MinimumLength(4)
                .Matches(ValidationConstants.AlphaNumeric)
                .WithMessage(string.Format(ValidationConstants.AlphaNumericMessage, "Region"));

        }
    }
}