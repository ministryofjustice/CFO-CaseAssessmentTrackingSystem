using Cfo.Cats.Domain.Labels;
using Cfo.Cats.Domain.ParticipantLabels;

namespace Cfo.Cats.Application.Features.ParticipantLabels.AddParticipantLabel;

public class AddParticipantLabelCommandHandler(ILabelRepository labelRepository, IParticipantLabelRepository participantLabelRepository, IParticipantLabelsCounter counter) : IRequestHandler<AddParticipantLabelCommand, Result>
{
    public async Task<Result> Handle(AddParticipantLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await labelRepository.GetByIdAsync( request.LabelId );

        if (label == null)
        {
            return Result.Failure("Invalid label id");
        }
        
        var participantLabel = ParticipantLabel.Create(
            request.ParticipantId, 
            label,
            counter);

        await participantLabelRepository.AddAsync(participantLabel);
        
        return Result.Success();
    }
}