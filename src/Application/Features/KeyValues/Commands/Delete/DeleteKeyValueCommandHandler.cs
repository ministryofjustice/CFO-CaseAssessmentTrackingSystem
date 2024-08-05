namespace Cfo.Cats.Application.Features.KeyValues.Commands.Delete;

public class DeleteKeyValueCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteKeyValueCommand, Result<int>>
{
    public async Task<Result<int>> Handle(DeleteKeyValueCommand request, CancellationToken cancellationToken)
    {
        var items = await unitOfWork.DbContext.KeyValues.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            var changeEvent = new KeyValueUpdatedDomainEvent(item);
            item.AddDomainEvent(changeEvent);
            unitOfWork.DbContext.KeyValues.Remove(item);
        }
        return Result<int>.Success(items.Count);
    }
}