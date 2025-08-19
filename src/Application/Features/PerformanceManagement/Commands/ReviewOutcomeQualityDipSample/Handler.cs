namespace Cfo.Cats.Application.Features.ManagementInformation.Commands.ReviewOutcomeQualityDipSample;

internal class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IRequestHandler<Command, Result>
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
