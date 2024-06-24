using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Documents.Queries;


public static class GetDocumentById
{
    [RequestAuthorize(Policy = PolicyNames.AuthorizedUser)]

    public class Query : IRequest<DownloadDocumentDto>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, DownloadDocumentDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUploadService _uploadService;
        
        public Handler(IApplicationDbContext context, IUploadService uploadService)
        {
            _context = context;
            _uploadService = uploadService;
        }

        public async Task<DownloadDocumentDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents.FindAsync(request.Id);
            var stream = await _uploadService.DownloadAsync(document!.URL!);
            DownloadDocumentDto dto = new DownloadDocumentDto()
            {
                FileStream = stream,
                FileExtension = "pdf",
                FileName = document.Title!
            };
            return dto;
        }
    }
}
