using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetExternalIdentifiers
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IRequest<ExternalIdentifierDto[]>
    {
        public string? ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, ExternalIdentifierDto[]>
    {
        public async Task<ExternalIdentifierDto[]> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = from p in unitOfWork.DbContext.Participants
                from ei in p.ExternalIdentifiers
                        where p.Id == request.ParticipantId
                select ei;

            var data = query.ProjectTo<ExternalIdentifierDto>(mapper.ConfigurationProvider);
                
            return await data.ToArrayAsync(cancellationToken);
        }
    }
}