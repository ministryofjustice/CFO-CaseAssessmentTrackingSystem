namespace Cfo.Cats.Application.Features.ManagementInformation.Commands.FinaliseOutcomeQualityDipSample;

public class Handler(IUnitOfWork unitOfWork, ICurrentUserService currentUser) : IRequestHandler<Command, Result>
{
    public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
    {
        var sample = await unitOfWork.DbContext.OutcomeQualityDipSamples
            .SingleAsync(s => s.Id == request.SampleId, cancellationToken);

        sample.Finalise(currentUser.UserId!);

        return Result.Success();
    }
}