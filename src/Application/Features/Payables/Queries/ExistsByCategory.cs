using Cfo.Cats.Application.Common.Security;
using Cfo.Cats.Application.SecurityConstants;

namespace Cfo.Cats.Application.Features.Payables.Queries;

public static class ExistsByCategory
{
    [RequestAuthorize(Policy = SecurityPolicies.Enrol)]
    public class Query : IRequest<bool>
    {
        public required string ParticipantId { get; set; }
        public required ActivityCategory Category { get; set; }
        public DateTime? AddedOn { get; set; }
    }

    class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, bool>
    {
        public async Task<bool> Handle(Query request, CancellationToken cancellationToken)
        {
            return await unitOfWork.DbContext.Activities
                .AnyAsync(a => a.ParticipantId == request.ParticipantId 
                    && a.Category == request.Category
                    && (request.AddedOn == null || request.AddedOn.Value.Date == a.Created!.Value.Date), cancellationToken); 
        }
    }

}
