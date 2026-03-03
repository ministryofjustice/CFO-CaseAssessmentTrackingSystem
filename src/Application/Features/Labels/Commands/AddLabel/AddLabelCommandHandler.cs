using Cfo.Cats.Domain.Labels;

namespace Cfo.Cats.Application.Features.Labels.Commands;

public class AddLabelCommandHandler(
        ILabelRepository repository, 
        ILabelCounter labelCounter) : IRequestHandler<AddLabelCommand, Result>
{
    public async Task<Result> Handle(
        AddLabelCommand request, 
        CancellationToken cancellationToken)
    {
        var l = Label.Create(
            request.Name, 
            request.Description,
            request.Scope, 
            request.Colour, 
            request.Variant, 
            request.AppIcon,
            request.ContractId, 
            labelCounter);
        await repository.AddAsync(l);
        return Result.Success();
    }
}
