using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.Features.Participants.DTOs;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Participants.Queries;

public class GetParticipantRisk
{
    [RequestAuthorize(Policy = PolicyNames.AuthorizedUser)]
    public class Query : IRequest<Result<RiskDto>>
    {
        public required string ParticipantId { get; set; }
        public Guid? RiskId { get; set; }
    }

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<Query, Result<RiskDto>>
    {
        public async Task<Result<RiskDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = unitOfWork.DbContext.Risks
                .Where(x => x.ParticipantId == request.ParticipantId);

            if(request.RiskId is not null)
            {
                query = query
                    .Where(x => x.Id == request.RiskId);
            }

            var risk = await query.OrderByDescending(x => x.Created)
                .FirstOrDefaultAsync();

            if(risk is null)
            {
                return Result<RiskDto>.Failure(["Risk not found."]);
            }

            return mapper.Map<RiskDto>(risk);
        }
    }
}
