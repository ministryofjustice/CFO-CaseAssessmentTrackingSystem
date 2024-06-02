namespace Cfo.Cats.Application.Features.KeyValues.Commands.Delete;

public class DeleteKeyValueCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteKeyValueCommand, Result<int>>
{

    public async Task<Result<int>> Handle(DeleteKeyValueCommand request, CancellationToken cancellationToken)
    {
        var items = await context.KeyValues.Where(x => request.Id.Contains(x.Id)).ToListAsync(cancellationToken);
        foreach (var item in items)
        {
            var changeEvent = new KeyValueUpdatedEvent(item);
            item.AddDomainEvent(changeEvent);
            context.KeyValues.Remove(item);
        }

        var result = await context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }
}