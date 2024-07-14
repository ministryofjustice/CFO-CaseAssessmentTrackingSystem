namespace Cfo.Cats.Application.Features.KeyValues.Commands.AddEdit;

public class AddEditKeyValueCommandHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper
) : IRequestHandler<AddEditKeyValueCommand, Result<int>>
{

    public async Task<Result<int>> Handle(AddEditKeyValueCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var keyValue = await unitOfWork.DbContext.KeyValues.FindAsync(new object[] { request.Id }, cancellationToken);
            _ = keyValue ?? throw new NotFoundException($"KeyValue Pair  {request.Id} Not Found.");
            keyValue = mapper.Map(request, keyValue);
            keyValue.AddDomainEvent(new KeyValueUpdatedDomainEvent(keyValue));
            return await Result<int>.SuccessAsync(keyValue.Id);
        }
        else
        {
            var keyValue = mapper.Map<KeyValue>(request);
            keyValue.AddDomainEvent(new KeyValueUpdatedDomainEvent(keyValue));
            unitOfWork.DbContext.KeyValues.Add(keyValue);
            return await Result<int>.SuccessAsync(keyValue.Id);
        }
    }
}
