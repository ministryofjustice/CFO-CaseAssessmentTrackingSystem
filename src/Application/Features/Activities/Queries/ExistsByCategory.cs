using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Activities.Queries;

public static class ExistsByCategory
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<bool>
    {
        public required string ParticipantId { get; set; }
        public required ActivityCategory Category { get; set; }
        public DateTime? CommencedOn { get; set; }
    }

    private class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, bool>
    {
        public async Task<bool> Handle(Query request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.Activities
                .AnyAsync(a => a.ParticipantId == request.ParticipantId 
                    && a.Category == request.Category
                    && (request.CommencedOn == null || request.CommencedOn.Value.Date == a.CommencedOn.Date), cancellationToken); 
        }
    }

}
