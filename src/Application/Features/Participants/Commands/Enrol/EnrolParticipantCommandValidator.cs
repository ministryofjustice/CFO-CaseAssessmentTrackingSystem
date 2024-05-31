using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using Hangfire.Dashboard.Resources;

namespace Cfo.Cats.Application.Features.Participants.Commands.Enrol;

public class EnrolParticipantCommandValidator : AbstractValidator<EnrolParticipantCommand>
{
    public EnrolParticipantCommandValidator(IApplicationDbContext dbContext)
    {

        RuleFor(x => x.Identifier).NotNull()
            .NotEmpty()
            .MinimumLength(9)
            .MaximumLength(9)
            .WithMessage("Invalid Cats Identifier");

        RuleFor(x => x.ReferralSource)
            .NotNull()
            .NotEmpty();

        When(x => x.ReferralSource is "Other" or "Healthcare", () => {
            RuleFor(x => x.ReferralComments)
                .NotNull()
                .NotEmpty()
                .WithMessage("Comments are mandatory with this referral source");
        });

    }

}