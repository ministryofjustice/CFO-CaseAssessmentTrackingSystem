using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public class DeleteLabelCommandHandler(
        ILabelRepository repository,
        ILabelCounter labelCounter) : IRequestHandler<DeleteLabelCommand, Result>
{
    public async Task<Result> Handle(
        DeleteLabelCommand request,
        CancellationToken cancellationToken)
    {
        var label = await repository.GetByIdAsync(request.LabelId);
        label.Delete(request.UserProfile.AsDomainUser(), labelCounter);
        return Result.Success();
    }
}
