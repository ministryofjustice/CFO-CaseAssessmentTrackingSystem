using Cfo.Cats.Domain.ParticipantLabels;

namespace Cfo.Cats.Application.Features.ParticipantLabels.CloseLabel;

public class CloseParticipantLabelCommandHandler(IParticipantLabelRepository labelRepository) : IRequestHandler<CloseParticipantLabelCommand, Result>
{
    public async Task<Result> Handle(CloseParticipantLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await labelRepository.GetByIdAsync(request.ParticipantLabelId);
        label.Close();
        return Result.Success();
    }
}