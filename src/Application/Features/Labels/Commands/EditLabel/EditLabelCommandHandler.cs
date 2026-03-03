using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public class EditLabelCommandHandler(ILabelRepository repository, ILabelCounter labelCounter) : IRequestHandler<EditLabelCommand, Result>
{
    public async Task<Result> Handle(EditLabelCommand request, CancellationToken cancellationToken)
    {
        var label = await repository.GetByIdAsync(request.LabelId);
        label.Edit(request.NewName, request.NewDescription, request.NewScope, request.NewColour, request.NewVariant, request.NewAppIcon, labelCounter);
        return Result.Success();
    }
}
