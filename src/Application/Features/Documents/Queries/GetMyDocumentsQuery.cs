using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Documents.DTOs;
using Cfo.Cats.Application.SecurityConstants;
using Cfo.Cats.Domain.Entities.Documents;

namespace Cfo.Cats.Application.Features.Documents.Queries;

public class GetMyDocumentsQuery
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]

    public class Query : IRequest<Result<GeneratedDocumentDto[]>> { }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<Query, Result<GeneratedDocumentDto[]>>
    {
        public async Task<Result<GeneratedDocumentDto[]>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.DbContext.GeneratedDocuments
                .Where(d => d.CreatedBy == currentUserService.UserId)
                .Where(d => DateTime.UtcNow < d.ExpiresOn); // Hide expired documents

            var documents = await query.ProjectTo<GeneratedDocumentDto>(mapper.ConfigurationProvider)
                .OrderByDescending(d => d.Created)
                .ToArrayAsync(cancellationToken) ?? [];

            return Result<GeneratedDocumentDto[]>.Success(documents);
        }
    }

}
