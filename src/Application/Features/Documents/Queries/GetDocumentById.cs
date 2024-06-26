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

    public class Handler(IApplicationDbContext context, IUploadService uploadService) : IRequestHandler<Query, DownloadDocumentDto>
    {
        public async Task<DownloadDocumentDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var document = await context.Documents.FindAsync(request.Id);
            var stream = await uploadService.DownloadAsync(document!.URL!);
            DownloadDocumentDto dto = new DownloadDocumentDto()
            {
                FileStream = stream,
                FileExtension = document.Title!.Split(".").Last(),
                FileName = document.Title!
            };
            return dto;
        }
    }
}
