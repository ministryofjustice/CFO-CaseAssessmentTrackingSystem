
using Cfo.Cats.Domain.Entities.Documents;
using Cfo.Cats.Domain.Events;

namespace Cfo.Cats.Application.Features.Documents.Commands.AddEdit;

public class AddEditDocumentCommandHandler : IRequestHandler<AddEditDocumentCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUploadService _uploadService;

    public AddEditDocumentCommandHandler(
        IApplicationDbContext context,
        IMapper mapper,
        IUploadService uploadService
    )
    {
        _context = context;
        _mapper = mapper;
        _uploadService = uploadService;
    }

    public async Task<Result<Guid>> Handle(AddEditDocumentCommand request, CancellationToken cancellationToken)
    {
        if (request.Id != Guid.Empty)
        {
            var document = await _context.Documents.FindAsync(new object[] { request.Id }, cancellationToken);
            _ = document ?? throw new NotFoundException($"Document {request.Id} Not Found.");
            document.AddDomainEvent(new DocumentUpdatedDomainEvent(document));
            if (request.UploadRequest != null)
            {
                document.URL = await _uploadService.UploadAsync(request.UploadRequest);
            }

            document.Title = request.Title;
            document.Description = request.Description;
            document.IsPublic = request.IsPublic;
            document.DocumentType = request.DocumentType;
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<Guid>.SuccessAsync(document.Id);
        }
        else
        {
            var document = _mapper.Map<Document>(request);
            if (request.UploadRequest != null)
            {
                document.URL = await _uploadService.UploadAsync(request.UploadRequest);
            }

            document.AddDomainEvent(new DocumentCreatedDomainEvent(document));
            _context.Documents.Add(document);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<Guid>.SuccessAsync(document.Id);
        }
    }
}