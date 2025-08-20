using Cfo.Cats.Application.Common.Interfaces;
using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Common.Validators;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.PerformanceManagement.Commands;

public static class ReviewDipSampleOutcome
{
    [RequestAuthorize(Policy = SecurityPolicies.OutcomeQualityDipReview)]
    public class Command : IRequest<Result>
    {
        public required Guid SampleId { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IRequestHandler<Command, Result>
    {
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var sample = await unitOfWork.DbContext.OutcomeQualityDipSamples
                    .Include(p => p.Participants)
                    // we can use first here as the validator stops null entries
                    .FirstAsync(s => s.Id == request.SampleId, cancellationToken);

            sample.Review(currentUser.UserId!);
            return Result.Success();
        }
    }

    private class Validator : AbstractValidator<Command>
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
}