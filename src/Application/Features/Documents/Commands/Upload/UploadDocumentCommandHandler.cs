namespace Cfo.Cats.Application.Features.Documents.Commands.Upload;

public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, Result<Guid>>
{
    private IApplicationDbContext context;

    public UploadDocumentCommandHandler(IApplicationDbContext context)
    {
        this.context = context;
    }


    public Task<Result<Guid>> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
    {
        return Result<Guid>.FailureAsync("Upload is not configured yet me old mukka");
    }
}