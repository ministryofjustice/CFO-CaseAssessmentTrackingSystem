using Cfo.Cats.Application.Common.Validators;

namespace Cfo.Cats.Application.Features.ManagementInformation.Commands.ReviewOutcomeQualityDipSample;

internal class Validator : AbstractValidator<Command>
{
    public Validator(IUnitOfWork unitOfWork) 
    {
        RuleFor(c => c.SampleId)
               .NotEmpty()
               .WithMessage("SampleId is required");

        RuleSet(ValidationConstants.RuleSet.MediatR, () =>
        {
            RuleFor(c => c)
                .MustAsync(async (_, command, context, token) =>
                {
                    var sample = await unitOfWork.DbContext.OutcomeQualityDipSamples
                    .Select(s => new
                    {
                        s.Status
                    })
                    .FirstOrDefaultAsync(token);

                    if (sample == null)
                    {
                        context.MessageFormatter.AppendArgument("Reason", "not found");
                        return false;
                    }

                    if (sample.Status == DipSampleStatus.AwaitingReview)
                    {
                        context.MessageFormatter.AppendArgument("Invalid Status", $"Cannot review at {sample.Status}");
                        return false;
                    }

                    return true;
                })
                .WithMessage("Cannot review: {Reason}");
        });

    }
}
