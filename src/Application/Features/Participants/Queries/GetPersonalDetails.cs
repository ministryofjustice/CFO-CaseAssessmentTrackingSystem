using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public static class GetPersonalDetails
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<ParticipantPersonalDetailDto?>
    {
        public required string ParticipantId { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, ParticipantPersonalDetailDto?>
    {
        public async Task<ParticipantPersonalDetailDto?> Handle(Query request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.Participants
                .Where(p => p.Id == request.ParticipantId)
                .Select(p => p.PersonalDetail)
                .ProjectTo<ParticipantPersonalDetailDto>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(cancellationToken);
        }
    }
}
