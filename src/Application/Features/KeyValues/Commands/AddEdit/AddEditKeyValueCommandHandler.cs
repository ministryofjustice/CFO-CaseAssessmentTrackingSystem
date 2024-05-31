namespace Cfo.Cats.Application.Features.KeyValues.Commands.AddEdit;

public class AddEditKeyValueCommandHandler : IRequestHandler<AddEditKeyValueCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AddEditKeyValueCommandHandler(
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<int>> Handle(AddEditKeyValueCommand request, CancellationToken cancellationToken)
    {
        if (request.Id > 0)
        {
            var keyValue = await _context.KeyValues.FindAsync(new object[] { request.Id }, cancellationToken);
            _ = keyValue ?? throw new NotFoundException($"KeyValue Pair  {request.Id} Not Found.");
            keyValue = _mapper.Map(request, keyValue);
            keyValue.AddDomainEvent(new UpdatedEvent<KeyValue>(keyValue));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(keyValue.Id);
        }
        else
        {
            var keyValue = _mapper.Map<KeyValue>(request);
            keyValue.AddDomainEvent(new UpdatedEvent<KeyValue>(keyValue));
            _context.KeyValues.Add(keyValue);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(keyValue.Id);
        }
    }
}
