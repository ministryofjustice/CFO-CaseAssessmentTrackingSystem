using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetContactDetails
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<IEnumerable<ParticipantContactDetailDto>>
    {
        public required string ParticipantId { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, IEnumerable<ParticipantContactDetailDto>>
    {
        public async Task<IEnumerable<ParticipantContactDetailDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.ParticipantContactDetails
                .Where(pcd => pcd.ParticipantId == request.ParticipantId)
                .ProjectTo<ParticipantContactDetailDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
        }
    }

}
