using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetContactDetails
{
    [RequestAuthorize(Policy = SecurityPolicies.AuthorizedUser)]
    public class Query : IQuery<IEnumerable<ParticipantContactDetailDto>>
    {
        public required string ParticipantId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IQueryHandler<Query, IEnumerable<ParticipantContactDetailDto>>
    {
        private static readonly IReadOnlyList<ConsentStatus> Granted = [ConsentStatus.GrantedStatus];

        public async Task<IEnumerable<ParticipantContactDetailDto>> Handle(Query request, CancellationToken cancellationToken) =>
            await unitOfWork.DbContext.ParticipantContactDetails
                .Where(pcd => pcd.ParticipantId == request.ParticipantId
                              && unitOfWork.DbContext.Participants.Any(p => p.Id == pcd.ParticipantId
                                  && Granted.Contains(p.ConsentStatus!)))
                .ProjectTo<ParticipantContactDetailDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
    }
}
